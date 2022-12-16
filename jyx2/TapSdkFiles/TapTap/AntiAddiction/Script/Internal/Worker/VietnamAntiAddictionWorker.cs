using System;
using System.Threading.Tasks;
using TapSDK.UI;
using TapTap.AntiAddiction.Internal;
using TapTap.AntiAddiction.Model;
using TapTap.Common;
using UnityEngine;

namespace TapTap.AntiAddiction
{
    public sealed class VietnamAntiAddictionWorker : BaseAntiAddictionWorker
    {
        /// <summary>
        /// 单日最长游玩时间(秒)
        /// </summary>
        private const int MAX_REMAIN_TIME_ONE_DAY = 180 * 60;
        
        /// <summary>
        /// 越南初始化检查可玩性时的时间
        /// </summary>
        private DateTime? _initCheckPlayableTimeVietnam; 
        
        /// <summary>
        /// 游戏剩余时间,从服务器得到,如果不能从服务器得到,那么给予默认最长时间(180分钟)
        /// </summary>
        private int? _remainTimeCache;
        
        #region Abstract Override

        public override Region Region => Region.Vietnam;

        private readonly string rsaPublicKey =
            "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA708T4I+a6wkvT7kn16HY9VrpBE3ay8/nNtaLVFNj/LVBVB6LyIHdU+XIIwi9nj9+I1+a0R2lBds6yKDy6uiwctAwhEHFcKKMmjbdfL0db8bACflASNdrAodw38i7SjzdDrlFiFvJiktkUWnSswaLLPpan/1K3fKo5GgzBtQd8Fe4GQYJ5ghePjA4vVHrpI5rBa9Ca0Ji2YnSOwYv9lFljMCKDYoTzn/Ctsq5vzN+ZGomjz+cATIbA8+zSek+XoGltZvQEWyBtjHDK/pkzb58Kc0zAnEmMQPPtHa0pCU1moMXKIiPvw+YXEVxyvOCUKLAHnzhJNTPlzZzKWtz9VGktQIDAQAB";

        internal override string RsaPublicKey => rsaPublicKey;
        
        protected override async Task<int> CompleteVerificationWindowAsync()
        {
            var userId = TapTapAntiAddictionManager.UserId;
            do 
            {
                try 
                {
                    VerificationResult result = await OpenVerificationPanelVietnam();
                    await UIManager.Instance.OpenToastAsync(Config.Current.UIConfig.InputRealNameInfoVietnam.submitSuccessMsg);
                    await Verification.Save(userId, Region.Vietnam, result);
                    if (!Verification.IsVerified)
                    {
                        // 如果在实名认证界面得到是未认证的数据，则弹出认证中的弹框
                        continue;
                    }
                    break;
                } 
                catch (TaskCanceledException) 
                {
                    TapLogger.Debug("Close verification panel.");
                    // 返回关闭实名制窗口
                    return StartUpResult.REAL_NAME_STOP;
                } 
                catch (Exception e) 
                {
                    // 其他异常
                    TapLogger.Error(e);
                    // 加载异常，则提示重新查询
                    await UIManager.Instance.OpenToastAsync(TapTapAntiAddictionManager.LocalizationItems.Current.NetError);
                }
            } 
            while (true);

            return 0;
        }

        protected override PlayableResult CheckOfflineMinorPlayable()
        {
            // 未成年人: 是否超过180分钟
            if (_remainTimeCache == null)
                _remainTimeCache = MAX_REMAIN_TIME_ONE_DAY;
            bool canPlay = true;
            if (_initCheckPlayableTimeVietnam != null)
            {
                DateTime now = DateTime.Now;
                // 是否跨天,跨天的话,重置当天可玩时间
                bool isCrossDay = _initCheckPlayableTimeVietnam.Value.Day != now.Day;
                if (isCrossDay)
                {
                    _initCheckPlayableTimeVietnam = new DateTime(now.Year, now.Month, now.Day);
                    _remainTimeCache = MAX_REMAIN_TIME_ONE_DAY;
                }
                DateTime endTime = now;
                DateTime beginTime = _initCheckPlayableTimeVietnam.Value;
                var timeSpan = endTime - beginTime;
                _remainTimeCache -= (int)timeSpan.TotalSeconds;
                canPlay = _remainTimeCache > 0;
                _remainTimeCache = Math.Max(0, _remainTimeCache.Value);
            }
            HealthReminderTip tip = canPlay ? null : Config.GetMinorUnplayableHealthReminderTipVietnam();
            return new PlayableResult 
            {
                RestrictType = PlayableResult.TIME_LIMIT,
                CanPlay = canPlay,
                Title = tip?.Title,
                Content = tip?.Content,
                RemainTime = _remainTimeCache.Value
            };
        }
        
        internal override Task<bool> IsStandaloneEnabled()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(false);
            return tcs.Task;
        }
        
        #endregion
        
        #region Virtual Override
        
        /// <summary>
        /// 检查可玩性后,已知未成年人时的处理
        /// </summary>
        /// <param name="playable"></param>
        /// <returns></returns>
        protected override async Task<int> OnCheckedPlayableWithMinorAsync(PlayableResult playable)
        {
            var tcs = new TaskCompletionSource<int>();
            // 登录时,如果发现不可玩的话,那么健康弹窗面板的按钮代表切换账号
            Action onSwitch = null;
            if (playable.CanPlay) 
            {
                tcs.TrySetResult(StartUpResult.LOGIN_SUCCESS);
                TryStartPoll();
            } 
            else 
            {
                onSwitch = () => 
                {
                    Logout();
                    AntiAddictionUIKit.ExternalCallback?.Invoke(StartUpResult.SWITCH_ACCOUNT, null);
                };
                tcs.TrySetResult(StartUpResult.DURATION_LIMIT);
            }

            if (onSwitch != null)
            {
                TapTapAntiAddictionUIKit.OpenHealthReminderPanel(playable, null, onSwitch);
            }
            return await tcs.Task;
        }
        
        /// <summary>
        /// 轮询时,发现不可玩处理
        /// </summary>
        /// <param name="playable"></param>
        protected override void OnUnplayablePostPoll(PlayableResult playable)
        {
            Action onExitGame = Application.Quit;
            TapTapAntiAddictionUIKit.OpenHealthReminderPanel(playable, onExitGame);
            AntiAddictionUIKit.ExternalCallback?.Invoke(StartUpResult.DURATION_LIMIT, null);
        }
        
        protected override async Task<PlayableResult> CheckPlayableAsync()
        {
            if (_initCheckPlayableTimeVietnam == null)
                _initCheckPlayableTimeVietnam = DateTime.Now;
            var result = await base.CheckPlayableAsync();
            if (_remainTimeCache == null)
                _remainTimeCache = result.RemainTime;
            return result;
        }
        
        internal override async Task<PlayableResult> CheckPlayableOnPollingAsync()
        {
            PlayableResult playable = await base.CheckPlayableOnPollingAsync();
            _initCheckPlayableTimeVietnam = DateTime.Now;
            _remainTimeCache = playable.RemainTime;

            return playable;
        }
        
        internal override void Logout()
        {
            base.Logout();
            _initCheckPlayableTimeVietnam = null;
            _remainTimeCache = null;
        }
        
        #endregion
        
        /// <summary>
        /// 打开越南实名制窗口
        /// </summary>
        /// <returns></returns>
        private  Task<VerificationResult> OpenVerificationPanelVietnam()
        {
            var tcs = new TaskCompletionSource<VerificationResult>();
           var path = AntiAddictionConst.GetPrefabPath(AntiAddictionConst.TIME_SELECTOR_PANEL_NAME,
               TapTapAntiAddictionManager.IsUseMobileUI());
            var timeSelectorPanel =
                UIManager.Instance.OpenUI<TaptapAntiAddictionTimeSelectorController>(path);
            if (timeSelectorPanel == null) return tcs.Task;
            timeSelectorPanel.OnVerified = (verification) => tcs.TrySetResult(verification);
            timeSelectorPanel.OnException = () => tcs.TrySetException(new Exception("Vietnam Verification Failed"));
            timeSelectorPanel.OnClosed = () => tcs.TrySetCanceled();

            return tcs.Task;
        }
    }
}