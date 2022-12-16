using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using Localization.AntiAddiction;
using TapSDK.UI;
using TapTap.AntiAddiction.Model;
using TapTap.AntiAddiction.Internal.Http;
using UnityEngine;
using Network = TapTap.AntiAddiction.Internal.Network;

namespace TapTap.AntiAddiction 
{
    public class TapTapAntiAddictionManager
    {
        internal static string GameId => AntiAddictionConfig.gameId;

        internal static string UserId;

        private static AntiAddictionConfig config;
        
        private static bool needResumePoll;

        public static AntiAddictionConfig AntiAddictionConfig => config == null ? config = new AntiAddictionConfig() : config;

        internal static string GetRsaPublicKey() => Worker.RsaPublicKey;

        public static AntiAddictionLocalizationItems LocalizationItems
        {
            get
            {
                if (_antiAddictionLocalizationItems == null)
                {
                    LocalizationMgr.Instance.SetLanguageType(AntiAddictionConfig.region == Region.China ? ELanguageType.cn : ELanguageType.en);
                    var textAsset = Resources.Load<TextAsset>(AntiAddictionLocalizationItems.PATH);
                    _antiAddictionLocalizationItems = AntiAddictionLocalizationItems.FromJson(textAsset.text);
                }
                return _antiAddictionLocalizationItems;
            }
        }
        private static AntiAddictionLocalizationItems _antiAddictionLocalizationItems;

        private static PlayableResult _currentPlayableResult;
        internal static PlayableResult CurrentPlayableResult
        {
            get => _currentPlayableResult;
            set
            {
                if (CurrentRemainSeconds == null)
                {
                    AntiAddictionPoll.StartCountdownRemainTime();
                }

                if (value != null)
                {
                    CurrentRemainSeconds = value.RemainTime;
                }
                _currentPlayableResult = value;
            }
        }

        internal static int? CurrentRemainSeconds {get; set; }

        /// <summary>
        /// 初始化, 建议使用这个接口,因为默认调用了 SetRegion,如果使用另一个 Init 接口,需要在 Init 之后,手动调用 SetRegion
        /// </summary>
        /// <param name="config"></param>
        public static void Init(AntiAddictionConfig config) 
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Init(config.gameId, config.showSwitchAccount);
            SetRegion(config.region);
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="showSwitchAccount"></param>
        public static void Init(string gameId,
            bool showSwitchAccount = true) 
        {
            if (string.IsNullOrEmpty(gameId)) 
            {
                throw new ArgumentNullException(nameof(gameId));
            }

            AntiAddictionConfig.showSwitchAccount = showSwitchAccount;
            
            Network.SetGameId(gameId);
        }
        
        /// <summary>
        /// 设置地区 (在启动之前设置)
        /// </summary>
        /// <param name="region"></param>
        public static void SetRegion(Region region)
        {
            Network.ChangeRegion(region);
            Network.ChangeHost();
            
            if (region == AntiAddictionConfig.region) return;
            
            AntiAddictionConfig.region = region;
            
            LocalizationMgr.Instance.SetLanguageType(region == Region.China ? ELanguageType.cn : ELanguageType.en);
            var textAsset = Resources.Load<TextAsset>(AntiAddictionLocalizationItems.PATH);
            _antiAddictionLocalizationItems = AntiAddictionLocalizationItems.FromJson(textAsset.text);
        }
        
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <returns></returns>
        public static async Task<int> StartUp( string userId)
        {
            if (string.IsNullOrEmpty(userId)) 
            {
                throw new ArgumentNullException(nameof(userId));
            }
            
            if (AntiAddictionConfig.useTapLogin)
            {
                throw new NotImplementedException();
            }
            
            UserId = Uri.EscapeDataString(userId);
            return await FetchVerification();
        }
        
        /// <summary>
        /// 获取身份验证状态
        /// </summary>
        /// <returns></returns>
        private static async Task<int> FetchVerification()
        {
            var worker = Worker;
            if (worker == null)
            {
                throw new ArgumentException($"Region {AntiAddictionConfig.region} is out of range!");
            }

            UIManager.Instance.OpenLoading();
            
            await worker.FetchConfigAsync();
            worker.OnConfigFetched();

            await worker.FetchVerificationAsync();
            return await worker.OnVerificationFetched();
        }

        /// <summary>
        /// 登出
        /// </summary>
        public static void Logout() 
        {
            CurrentRemainSeconds = null;
            CurrentPlayableResult = null;
            Worker.Logout();
        }

        /// <summary>
        /// 检查支付
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async Task<PayableResult> CheckPayLimit(long amount)
        {
            return await Worker.CheckPayableAsync(amount);
        }

        /// <summary>
        /// 上报支付
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Task SubmitPayResult(long amount) 
        {
            return Worker.SubmitPayResult(amount);
        }
        
        /// <summary>
        /// 轮询时,检查可玩性
        /// </summary>
        /// <returns></returns>
        internal static async Task<PlayableResult> CheckPlayableOnPolling()
        {
            CurrentPlayableResult = await Worker.CheckPlayableOnPollingAsync();
            return CurrentPlayableResult;
        }

        internal static async Task<bool> IsStandaloneEnabled()
        {
            return await Worker.IsStandaloneEnabled();
        }
        
        public static void EnterGame()
        {
            if (needResumePoll)
            {
                AntiAddictionPoll.StartUp();
                needResumePoll = false;
            }
        }

        public static void LeaveGame()
        {
            if (AntiAddictionPoll.StartPoll)
            {
                AntiAddictionPoll.Logout();
                needResumePoll = true;
            }
        }

        /// <summary>
        /// 是否使用移动版 UI,否则就是用 Standalone 版本 UI
        /// </summary>
        /// <returns></returns>
        public static bool IsUseMobileUI()
        {
            if (useMobileUI != null)
                return useMobileUI.Value;
            #if UNITY_ANDROID || UNITY_IOS
            return true;
            #else
            return false;
            #endif
        }
        
        public static bool? useMobileUI;
        
        #region Worker
        
        private static readonly Dictionary<Region, BaseAntiAddictionWorker>
            Workers = new Dictionary<Region, BaseAntiAddictionWorker>();

        private static BaseAntiAddictionWorker GetWorker(Region region)
        {
            if (Workers.TryGetValue(region, out BaseAntiAddictionWorker worker) && worker != null)
                return worker;
            BaseAntiAddictionWorker newWorker = null;
            switch (region)
            {
                case Region.China:
                    newWorker = new ChinaAntiAddictionWorker();
                    break;
                case Region.Vietnam:
                    newWorker = new VietnamAntiAddictionWorker();
                    break;
            }

            Workers[region] = newWorker;
            
            Debug.LogFormat($"Get New Worker! region: {region.ToString()} newWorker: {(newWorker != null ? newWorker.GetType().ToString() : "NULL")} worker In Dic: {(Workers[region] != null ? Workers[region].GetType().ToString() : "NULL")}");
            return newWorker;
        }

        private static BaseAntiAddictionWorker Worker => GetWorker(AntiAddictionConfig.region);

        #endregion
    }
}
