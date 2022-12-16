using System;
using TapTap.AntiAddiction.Model;
using MobileBridgeAntiAddiction = Plugins.AntiAddictionUIKit.AntiAddictionUIKit;
using CheckPayResult = Plugins.AntiAddictionUIKit.CheckPayResult;

namespace TapTap.AntiAddiction
{
    public sealed class AntiAddictionMobileOldJob : IAntiAddictionJob
    {
        private Action<int, string> _externalCallback;

        public Action<int, string> ExternalCallback
        {
            get => _externalCallback;
        }
        
        public int AgeRange => MobileBridgeAntiAddiction.CurrentUserAgeLimit();

        /// <summary>
        /// 剩余时间(单位:秒)
        /// </summary>
        public int RemainingTime => MobileBridgeAntiAddiction.CurrentUserRemainTime();
        
        /// <summary>
        /// 剩余时间(单位:分钟)
        /// </summary>
        public int RemainingTimeInMinutes
        {
            get
            {
                int seconds = RemainingTime;
                if (seconds <= 0) 
                    return 0;
                return UnityEngine.Mathf.CeilToInt(seconds / 60.0f);
            }
        }
        
        public string CurrentToken => MobileBridgeAntiAddiction.CurrentToken();

        private bool useTapLogin;
        
        public void Init(AntiAddictionConfig config, Action<int, string> callback)
        {
            _externalCallback = callback;
            useTapLogin = config.useTapLogin;
            MobileBridgeAntiAddiction.Init(config.gameId, true, true, config.showSwitchAccount
                , (data) => callback?.Invoke(data.code, null)
                , (data) => callback?.Invoke(-1, data));
        }
        
        public void Startup(string userId)
        {
            MobileBridgeAntiAddiction.Startup(useTapLogin, userId);
        }

        public void Exit()
        {
            MobileBridgeAntiAddiction.Logout();
        }

        public void EnterGame()
        {
            MobileBridgeAntiAddiction.EnterGame();
        }

        public void LeaveGame()
        {
            MobileBridgeAntiAddiction.LeaveGame();
        }

        public void CheckPayLimit(long amount, Action<CheckPayResult> handleCheckPayLimit, Action<string> handleCheckPayLimitException)
        {
            MobileBridgeAntiAddiction.CheckPayLimit(amount, handleCheckPayLimit, handleCheckPayLimitException);
        }

        public void SubmitPayResult(long amount, Action handleSubmitPayResult, Action<string> handleSubmitPayResultException)
        {
            MobileBridgeAntiAddiction.SubmitPayResult(amount, handleSubmitPayResult, handleSubmitPayResultException);
        }
        
        public bool isStandalone()
        {
            return MobileBridgeAntiAddiction.isStandalone();
        }
    }
}
