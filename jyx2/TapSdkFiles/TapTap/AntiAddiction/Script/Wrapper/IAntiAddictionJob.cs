using System;
using CheckPayResult = Plugins.AntiAddictionUIKit.CheckPayResult;
using TapTap.AntiAddiction.Model;

namespace TapTap.AntiAddiction
{
    internal interface IAntiAddictionJob
    {
        /// <summary>
        /// 根据状态的对外回调
        /// </summary>
        Action<int, string> ExternalCallback { get;}
        
        int AgeRange { get; }

        int RemainingTimeInMinutes { get; }

        /// <summary>
        /// 剩余时间(单位:秒)
        /// </summary>
        int RemainingTime { get; }
        
        string CurrentToken { get; }
        
        /// <summary>
        /// 新的初始化接口
        /// </summary>
        /// <param name="config"></param>
        /// <param name="callback">int 代表返回 code, string 代表 message</param>
        void Init(AntiAddictionConfig config, Action<int, string> callback);

        void Startup(string userId);

        void Exit();

        void EnterGame();

        void LeaveGame();
        
        void CheckPayLimit(long amount
            , Action<CheckPayResult> handleCheckPayLimit
            , Action<string> handleCheckPayLimitException);

        void SubmitPayResult(long amount
            , Action handleSubmitPayResult
            , Action<string> handleSubmitPayResultException);
        
        bool isStandalone();
    }
}
