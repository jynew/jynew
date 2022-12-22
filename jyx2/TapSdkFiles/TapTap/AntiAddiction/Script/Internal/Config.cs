using System;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using LC.Newtonsoft.Json;
using TapTap.AntiAddiction.Model;
using TapTap.Common;

namespace TapTap.AntiAddiction.Internal 
{
    internal static class Config 
    {
        static readonly string CONFIG_SDK_PATH = "Config/anti-addiction";
        internal static readonly string ANTI_ADDICTION_DIR = "tap-anti-addiction";
        static readonly string CONFIG_FILENAME = "config";
        static readonly string UPLOAD_USER_ACTION = "1";
        static readonly int USEABLE_HEALTH_REMINDER_INDEX = 1;
        static readonly int MINOR_UNPLAYABLE_HEALTH_REMINDER_INDEX = 5;
        static readonly int MINOR_PLAYABLE_HEALTH_REMINDER_INDEX = 7;
        static readonly int CHINA_IDENTIFY_INPUT_TIP_INDEX = 3;
        static readonly int CHINA_IDENTIFY_INPUT_BLOCK_TIP_INDEX = 4;
        static readonly int CHINA_IDENTIFY_INPUT_FORMAT_ERROR_TIP_INDEX = 5;
        private const string TIME_FROMAT = "HH:mm";

        internal static readonly Persistence persistence = new Persistence(Path.Combine(Application.persistentDataPath,
            ANTI_ADDICTION_DIR, CONFIG_FILENAME));
        

        static AntiAddictionConfigResult current;

        static Dictionary<int, HealthReminderTip> healthReminderTips;
        static Dictionary<int, PayLimitTip> payLimitTips;
        static Dictionary<int, AuthIdentifyTip> authIdentifyTips;
        internal static string healthPayTipButtonText;

        internal static AntiAddictionConfigResult Current 
        {
            get => current;
            private set =>  current = value;
        }

        internal static async Task Fetch() 
        {
            // 从服务端加载
            try 
            {
                Current = await Network.FetchConfig();
                if (IsValid())
                {
                    await persistence.Save(Current);
                    return;
                }
                else
                {
                    LoadFromBuiltin();
                }
            } 
            catch (Exception e) 
            {
                TapLogger.Error(e);
            }

            // 从设备加载
            try 
            {
                Current = await persistence.Load<AntiAddictionConfigResult>();
                if (Current != null)
                    return;
            } 
            catch (Exception e) 
            {
                TapLogger.Error(e);
            }

            LoadFromBuiltin();
        }

        private static void LoadFromBuiltin()
        {
            // 从 SDK 中加载
            try
            {
                TextAsset textAsset = Resources.Load(CONFIG_SDK_PATH) as TextAsset;
                if (textAsset != null)
                    Current = JsonConvert.DeserializeObject<AntiAddictionConfigResult>(textAsset.text);
            } 
            catch (Exception e) 
            {
                TapLogger.Error(e);
            }
        }

        private static bool IsValid()
        {
            if (current == null) return false;
            if (current.UIConfig == null) return false;
            return true;
        }

        internal static void OnFetched()
        {
            if (current == null) return;
            // 梳理数据，方便查找
            healthPayTipButtonText = current.UIConfig?.PaymentInfo?.buttonConfirm;
            healthReminderTips = new Dictionary<int, HealthReminderTip>();
            if (current.UIConfig?.HealthReminderTips != null)
            {
                foreach (HealthReminderTips tips in current.UIConfig.HealthReminderTips) 
                {
                    if (tips.AccountType == USEABLE_HEALTH_REMINDER_INDEX) 
                    {
                        foreach (HealthReminderTip tip in tips.Tips) 
                        {
                            healthReminderTips.Add(tip.Type, tip);
                        }
                    }
                }
            }
            payLimitTips = new Dictionary<int, PayLimitTip>();
            if (current.UIConfig?.PayLimitTips != null)
            {
                foreach (PayLimitTip tip in current.UIConfig.PayLimitTips) 
                {
                    payLimitTips.Add(tip.AccountType, tip);
                }
            }
            authIdentifyTips = new Dictionary<int, AuthIdentifyTip>();
            if (current.UIConfig?.AuthIdentifyTips != null)
            {
                foreach (var tip in current.UIConfig.AuthIdentifyTips) 
                {
                    authIdentifyTips.Add(tip.AuthIdentifyType, tip);
                }
            }
        }
        
        internal static bool NeedUploadUserAction => Current?.UploadUserAction == UPLOAD_USER_ACTION;

        internal static HealthReminderTip GetMinorUnplayableHealthReminderTip() 
        {
            if (healthReminderTips.TryGetValue(MINOR_UNPLAYABLE_HEALTH_REMINDER_INDEX, out HealthReminderTip tip)) 
            {
                return tip;
            }
            return null;
        }
        
        internal static HealthReminderTip GetMinorUnplayableHealthReminderTipVietnam()
        {
            var healthInfo = Current.UIConfig.HealthReminderVietnam;
            if (healthInfo != null)
            {
                return new HealthReminderTip
                {
                    Type = 1,
                    Title = healthInfo.title,
                    Content = healthInfo.description,
                };
            }
            return null;
        }

        internal static AuthIdentifyTip GetInputIdentifyTip()
        {
            if (authIdentifyTips.TryGetValue(CHINA_IDENTIFY_INPUT_TIP_INDEX, out AuthIdentifyTip tip)) 
            {
                return tip;
            }
            return null;
        }
        
        internal static AuthIdentifyTip GetInputIdentifyFormatErrorTip()
        {
            if (authIdentifyTips.TryGetValue(CHINA_IDENTIFY_INPUT_FORMAT_ERROR_TIP_INDEX, out AuthIdentifyTip tip)) 
            {
                return tip;
            }
            return null;
        }
        
        /// <summary>
        /// 认证中提示(因为中宣部认证无响应)
        /// </summary>
        /// <returns></returns>
        internal static AuthIdentifyTip GetInputIdentifyBlockingTip()
        {
            if (authIdentifyTips.TryGetValue(CHINA_IDENTIFY_INPUT_BLOCK_TIP_INDEX, out AuthIdentifyTip tip)) 
            {
                return tip;
            }
            return null;
        }
        
        internal static HealthReminderTip GetMinorPlayableHealthReminderTip() 
        {
            if (healthReminderTips.TryGetValue(MINOR_PLAYABLE_HEALTH_REMINDER_INDEX, out HealthReminderTip tip)) 
            {
                return tip;
            }
            return null;
        }

        internal static DateTimeOffset StrictStartTime =>
            DateTimeOffset.ParseExact(current.ChildProtectedConfig.NightStrictStart,
                TIME_FROMAT, CultureInfo.InvariantCulture);

        internal static DateTimeOffset StrictEndTime =>
            DateTimeOffset.ParseExact(current.ChildProtectedConfig.NightStrictEnd,
                TIME_FROMAT, CultureInfo.InvariantCulture);
    }
}
