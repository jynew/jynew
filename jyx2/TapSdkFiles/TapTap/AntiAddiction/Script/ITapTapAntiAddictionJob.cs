using System.Threading.Tasks;
using TapTap.AntiAddiction.Model;

namespace TapTap.AntiAddiction
{
    internal interface ITapTapAntiAddictionJob
    {
        /// <summary>
        /// 获取年龄，未登录状态返回0,并不是返回用户准确年龄，只是返回一个年龄区间，比如大于18岁，只会返回18
        /// </summary>
        int UserAgeLimit { get; }
        
        /// <summary>
        /// 获取用户剩余时长，未登录状态返回0
        /// </summary>
        int UserRemainTime { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">初始化配置,包括</param>
        void Init(AntiAddictionConfig config);

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="userId">玩家Id</param>
        /// <returns>参考TapTap.AntiAddiction.Model.StartUpResult</returns>
        Task<int> Login(string userId);
        
        /// <summary>
        /// 登出
        /// </summary>
        void Logout();
        
        /// <summary>
        /// 检查是否可以支付
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<PayableResult> CheckPayLimit(int amount);

        /// <summary>
        /// 上报支付
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task SubmitPayResult(long amount);

    }
}
