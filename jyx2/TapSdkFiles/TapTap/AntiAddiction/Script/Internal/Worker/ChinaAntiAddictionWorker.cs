using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TapSDK.UI;
using TapTap.AntiAddiction.Internal;
using TapTap.AntiAddiction.Model;
using TapTap.Common;
using UnityEngine;

namespace TapTap.AntiAddiction
{
    public sealed class ChinaAntiAddictionWorker : BaseAntiAddictionWorker
    {
        #region Abstract Override

        public override Region Region => Region.China;

        private readonly string rsaPublicKey =
            "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1pM6yfulomBTXWKiQT5gK9fY4hq11Kv8D+ewum25oPGReuEn6dez7ogA8bEyQlnYYUoEp5cxYPBbIxJFy7q1qzQhTFphuFzoC1x7DieTvfZbh+b60psEottrCD8M0Pa3h44pzyIp5U5WRpxRcQ9iULolGLHZXJr9nW6bpOsyEIFG5tQ7qCBj8HSFoNBKZH+5Cwh3j5cjmyg55WdJTimg9ysbbwZHYmI+TFPuGo/ckHT6j4TQLCmmxI8Qf5pycn3/qJWFhjx/y8zaxgn2hgxbma8hyyGRCMnhM5tISYQv4zlQF+5RashvKa2zv+FHA5DALzIsGXONeTxk6TSBalX5gQIDAQAB";

        internal override string RsaPublicKey => rsaPublicKey;

        private TaptapAntiAddictionIDInputController idInputPanel;

        protected override async Task<int> CompleteVerificationWindowAsync()
        {
            var userId = TapTapAntiAddictionManager.UserId;
            do 
            {
                try 
                {
                    var result = await OpenVerificationPanelCn();
                    await Verification.Save(userId, Region.China, result);
                    var tcs = new TaskCompletionSource<int>();
                    // 如果在实名认证界面得到是未认证的数据，则弹出认证中的弹框
                    if (Verification.IsVerifing)
                    {
                        var tip = Config.GetInputIdentifyBlockingTip();
                        
                        Action onOk = () => 
                        {
                            Debug.Log("因为认证中,退出游戏!");
                            Application.Quit();
                        };
                        
                        TapTapAntiAddictionUIKit.OpenHealthPaymentPanel(tip.Title, tip.Content, tip.PositiveButtonText, onOk);
                    }
                    else
                    {
                        if (Verification.IsVerified)
                            TapTapAntiAddictionUIKit.OpenVerifyFinishPanel(()=> tcs.TrySetResult(0));
                        else
                        {
                            idInputPanel.ShowErrorTip("认证未通过,请提交真实信息");
                            continue;
                        }
                    }
                    return await tcs.Task;
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
                    if (e.Message.Contains("Verification Failed"))
                    {
                        await TapTapAntiAddictionUIKit.ShowRetryDialog(Config.GetInputIdentifyFormatErrorTip()?.Content);
                    }
                    else
                    {
                        if (e is HttpRequestException || e is WebException)
                        {
                            idInputPanel.ShowErrorTip(TapTapAntiAddictionManager.LocalizationItems.Current
                                .NetError);
                            continue;
                        }

                        if (e is AntiAddictionException aae)
                        {
                            if (aae.Code == 3)
                            {
                                idInputPanel.ShowErrorTip(aae.Message);
                                continue;
                            }
                            if (aae.Code >= (int)HttpStatusCode.InternalServerError)
                            {
                                idInputPanel.ShowErrorTip("请求出错");
                                continue;
                            }
                        }
                        if (e.Message.Contains("Interval server error."))
                        {
                            idInputPanel.ShowErrorTip("请求出错");
                            continue;
                        }
                        idInputPanel.ShowErrorTip(TapTapAntiAddictionManager.LocalizationItems.Current.NoVerification);
                        await Task.Yield();
                    }
                }
            } 
            while (true);
        }

        internal override async Task<bool> IsStandaloneEnabled()
        {
            return await StandaloneChina.Enabled();
        }
        
        protected override PlayableResult CheckOfflineMinorPlayable()
        {
            // 未成年人
            if (IsGameTime()) 
            {
                // 可玩：节假日并且是游戏时间
                HealthReminderTip playableTip = Config.GetMinorPlayableHealthReminderTip();

                // 计算时间
                DateTimeOffset gameEndTime = Config.StrictStartTime;
                TimeSpan remain = gameEndTime - DateTimeOffset.Now;

                return new PlayableResult 
                {
                    RestrictType = PlayableResult.NIGHT_STRICT,
                    CanPlay = true,
                    Title = playableTip?.Title,
                    Content = playableTip?.Content,
                    RemainTime = Convert.ToInt32(remain.TotalSeconds)
                };
            }

            // 不可玩：未成年人不在可玩时间
            HealthReminderTip unplayableTip = Config.GetMinorUnplayableHealthReminderTip();
            return new PlayableResult 
            {
                RestrictType = PlayableResult.NIGHT_STRICT,
                CanPlay = false,
                Title = unplayableTip?.Title,
                Content = unplayableTip?.Content,
            };
        }

        #endregion
        
        /// <summary>
        /// 是否在允许游戏时间段
        /// </summary>
        private bool IsGameTime()
        {
            DateTimeOffset strictStart = Config.StrictStartTime;
            DateTimeOffset strictEnd = Config.StrictEndTime;

            bool playable;
            DateTimeOffset now = DateTimeOffset.Now;
            if (strictEnd > strictStart) 
            {
                // 可玩时间分段
                DateTimeOffset today = new DateTimeOffset(DateTime.Today);
                DateTimeOffset tomorrow = today.AddDays(1);
                playable = (now > today && now < strictStart) ||
                           (now > strictEnd && now < tomorrow);
            } 
            else 
            {
                playable = now > strictEnd && now < strictStart;
            }
            return playable;
        }

        /// <summary>
        /// 打开中国实名制窗口
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        private Task<VerificationResult> OpenVerificationPanelCn()

        {
            var tcs = new TaskCompletionSource<VerificationResult>();
            var path = AntiAddictionConst.GetPrefabPath(AntiAddictionConst.ID_NUMBER_INPUT_PANEL_NAME,
                false);
            idInputPanel =
                UIManager.Instance.OpenUI<TaptapAntiAddictionIDInputController>(path);
            if (idInputPanel != null)
            {
                idInputPanel.OnVerified = (verification) => tcs.TrySetResult(verification);
                idInputPanel.OnException = (e) =>
                {
                    if (e is HttpRequestException || e is WebException)
                    {
                        tcs.TrySetException(e);
                    }
                    else
                    {
                        if (e is AntiAddictionException aae)
                        {
                            // code == 3 代表身份证号码非法
                            if (aae.Code == (int)HttpStatusCode.Forbidden || aae.Code == 3 ||
                                aae.Code >= (int)HttpStatusCode.InternalServerError)
                                tcs.TrySetException(e);
                        }
                        else
                        {
                            if (e.Message.Contains("Interval server error."))
                                tcs.TrySetException(e);
                            else
                                tcs.TrySetException(new Exception("China Verification Failed"));
                        }
                    }
                };
                idInputPanel.OnClosed = () => tcs.TrySetCanceled();
            }

            return tcs.Task;
        }
    }
}