
namespace TapTap.AntiAddiction.Model 
{
    public static class StartUpResult
    {
        public const int INTERNAL_ERROR = -1;   // 内部错误
        public const int LOGIN_SUCCESS = 500;   // 登陆成功
    
        public const int EXITED = 1000;   //用户登出
        
        public const int SWITCH_ACCOUNT = 1001;   //切换账号
        
        public const int PERIOD_RESTRICT = 1030;   //用户当前无法进行游戏
        public const int DURATION_LIMIT = 1050;   //时长限制
        public const int REAL_NAME_STOP = 9002;   //实名过程中点击了关闭实名窗
        // 新增
        // public const int OPEN_WITH_TIP = 1095;   //未成年允许游戏弹窗
        // public const int VERIFIY_BLOCKED = 5001;    // 身份认证中被卡主(中国使用,比如中宣部认证无响应)
    }
}
