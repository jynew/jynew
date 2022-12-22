using System;
using UnityEngine;
using CheckPayResult = Plugins.AntiAddictionUIKit.CheckPayResult;
using TapTap.AntiAddiction.Model;

namespace TapTap.AntiAddiction
{
    public enum Region
    {
        // NOTE:不要随便改枚举的int值,已经被序列化保存到本地过.
        China = 1,
        Vietnam = 2,
    }
    
    public static class AntiAddictionUIKit
    {
        private static IAntiAddictionJob _job;

        private static IAntiAddictionJob Job
        {
            get
            {
                if (_job == null)
                {
                    InitJob();
                }
                return _job;
            }
        }

        private static Region _region = Region.China;
        
        internal static Action<int, string> ExternalCallback
        {
            get => Job?.ExternalCallback;
        }

        private static void InitJob()
        {
            // 国内-移动端防沉迷用桥接的方式
            if (Region.China == _region && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                _job = new AntiAddictionMobileOldJob();
            }
            // 其他均使用 Unity Native 的方式
            else
            {
                _job = new AntiAddictionNewJob();
            }
            
            Debug.LogFormat("Anti Addiction Job Type: {0} ! Region: {1} Platform: {2}", _job.GetType(), _region.ToString(), Application.platform.ToString());
        }

        public static void SetRegion(Region region)
        {
            if (region != _region || _job == null)
            {
                _region = region;
                InitJob();
            }
            
            var newJob = Job as AntiAddictionNewJob;
            if (newJob != null)
            {
                newJob.SetRegion(region);
            }
        }
        
        public static void Init(AntiAddictionConfig config, Action<int, string> callback)
        {
            Job.Init(config, callback);
        }
        
        public static void Startup(string userId)
        {
            Job.Startup(userId);
        }

        public static void Exit()
        {
            Job.Exit();
        }

        public static void EnterGame()
        {
            Job.EnterGame();
        }

        public static void LeaveGame()
        {
            Job.LeaveGame();
        }
        
        public static int AgeRange
        {
            get
            {
                if (Job != null)
                    return Job.AgeRange;
                return 0;
            }
        }

        public static int RemainingTimeInMinutes
        {
            get
            {
                if (Job != null)
                    return Job.RemainingTimeInMinutes;
                return 0;
            }
        }

        /// <summary>
        /// 剩余时间(单位:秒)
        /// </summary>
        public static int RemainingTime
        {
            get
            {
                if (Job != null)
                    return Job.RemainingTime;
                return 0;
            }
        }

        public static string CurrentToken
        {
            get
            {
                if (Job != null)
                    return Job.CurrentToken;
                return "";
            }
        }
        
        /// <summary>
        /// 在支付前,检查支付结果
        /// </summary>
        /// <param name="amount">支付金额,单位:分</param>
        /// <param name="handleCheckPayLimit">检查支付结果的回调</param>
        /// <param name="handleCheckPayLimitException">检查支付碰到问题时的回调</param>
        public static void CheckPayLimit(long amount
            , Action<CheckPayResult> handleCheckPayLimit
            , Action<string> handleCheckPayLimitException)
        {
            Job.CheckPayLimit(amount, handleCheckPayLimit, handleCheckPayLimitException);
        }
        
        /// <summary>
        /// 提交支付结果
        /// </summary>
        /// <param name="amount">支付金额,单位:分</param>
        /// <param name="handleSubmitPayResult">提交成功后的回调</param>
        /// <param name="handleSubmitPayResultException">提交失败后的回调</param>
        public static void SubmitPayResult(long amount
            , Action handleSubmitPayResult
            , Action<string> handleSubmitPayResultException
        )
        {
            Job.SubmitPayResult(amount, handleSubmitPayResult, handleSubmitPayResultException);
        }
        
        public static bool isStandalone()
        {
            if (Job != null)
                return Job.isStandalone();
            return false;
        }
        
    }
}