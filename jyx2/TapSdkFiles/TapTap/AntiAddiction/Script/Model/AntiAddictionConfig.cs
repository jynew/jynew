using System.Collections.Generic;
using LC.Newtonsoft.Json;

namespace TapTap.AntiAddiction.Model
{
    internal class ChildProtectedConfig 
    {
        // 宵禁开始时间（作为可玩结束时间）
        [JsonProperty("night_strict_start")]
        internal string NightStrictStart { get; private set; }

        // 宵禁结束时间（作为可玩开始时间）
        [JsonProperty("night_strict_end")]
        internal string NightStrictEnd { get; private set; }
    }

    /// <summary>
    /// 充值限制
    /// </summary>
    internal class PayLimitTip 
    {
        /// <summary>
        /// 账号类型
        /// </summary>
        [JsonProperty("account_type")]
        internal int AccountType { get; private set; }

        /// <summary>
        /// 单笔充值标题
        /// </summary>
        [JsonProperty("single_title")]
        internal string SingleTitle { get; private set; }

        /// <summary>
        /// 单笔充值描述
        /// </summary>
        [JsonProperty("single_description")]
        internal string SingleContent { get; private set; }

        /// <summary>
        /// 单笔充值限额
        /// </summary>
        [JsonProperty("single_limit")]
        internal int SingleLimit { get; private set; }

        /// <summary>
        /// 当月充值标题
        /// </summary>
        [JsonProperty("month_title")]
        internal string MonthTitle { get; private set; }

        /// <summary>
        /// 当月充值描述
        /// </summary>
        [JsonProperty("month_description")]
        internal string MonthContent { get; private set; }

        /// <summary>
        /// 当月充值限额
        /// </summary>
        [JsonProperty("month_limit")]
        internal int MonthLimit { get; private set; }
    }

    /// <summary>
    /// 输入身份信息
    /// </summary>
    internal class AuthIdentifyTip
    {
        /// <summary>
        /// 授权类型
        /// </summary>
        [JsonProperty("type")]
        internal int AuthIdentifyType { get; private set; }

        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty("title")]
        internal string Title { get; private set; }      
        
        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty("description")]
        internal string Content { get; private set; }
        
        /// <summary>
        /// 否定按钮内容
        /// </summary>
        [JsonProperty("negative_button")]
        internal string NegativeButtonText { get; private set; }
        
        /// <summary>
        /// 否定按钮内容
        /// </summary>
        [JsonProperty("positive_button")]
        internal string PositiveButtonText { get; private set; }
    }
    
    /// <summary>
    /// 健康提示
    /// </summary>
    internal class HealthReminderTip 
    {
        [JsonProperty("type")]
        internal int Type { get; set; }

        [JsonProperty("title")]
        internal string Title { get; set; }

        [JsonProperty("description")]
        internal string Content { get; set; }
    }

    internal class HealthReminderTips 
    {
        [JsonProperty("account_type")]
        internal int AccountType { get; private set; }

        [JsonProperty("tips")]
        internal HealthReminderTip[] Tips { get; private set; }
    }
    
    internal class UIConfig 
    {
        [JsonProperty("pay_limit_words")]
        internal PayLimitTip[] PayLimitTips { get; private set; }

        [JsonProperty("health_reminder_words")]
        internal HealthReminderTips[] HealthReminderTips { get; private set; }
        
        /// <summary>
        /// 支付限制时的按钮提醒
        /// </summary>
        [JsonProperty("pay_reminder")]
        internal PaymentInfo PaymentInfo { get; private set; }
        
        /// <summary>
        /// 身份授权消息
        /// </summary>
        [JsonProperty("auth_identify_words")]
        internal AuthIdentifyTip[] AuthIdentifyTips { get; private set; }
        
        /// <summary>
        /// 输入用户信息提示(越南防沉迷使用)
        /// </summary>
        [JsonProperty("input_realname_info")]
        internal InputRealNameInfo InputRealNameInfoVietnam { get; private set; }
        
        /// <summary>
        /// 健康提示(越南防沉迷使用)
        /// </summary>
        [JsonProperty("health_reminder")]
        internal HealReminderVietnam HealthReminderVietnam { get; private set; }
    }

    internal class UIPanelConfig
    {
        [JsonProperty("title")]
        public string title;
        [JsonProperty("description")]
        public string description;
        [JsonProperty("button")]
        public string button;
    }
    
    internal class PaymentInfo
    {
        [JsonProperty("button_confirm")]
        public string buttonConfirm;
    }
    
    internal class InputRealNameInfo : UIPanelConfig
    {
        [JsonProperty("submit_success_message")]
        public string submitSuccessMsg;
        [JsonProperty("birthdate_invalidate_message")]
        public string invalidateMessage;
    }    
    
    internal class HealReminderVietnam
    {
        [JsonProperty("title")]
        public string title;
        [JsonProperty("description")]
        public string description;
        [JsonProperty("button_exit")]
        public string buttonExit;
        [JsonProperty("button_switch")]
        public string buttonSwitch;
    }

    public class AntiAddictionConfigResult 
    {
        /// <summary>
        /// 应用名
        /// </summary>
        [JsonProperty("name")]
        internal string Name { get; private set; }

        [JsonProperty("child_protected_config")]
        internal ChildProtectedConfig ChildProtectedConfig { get; private set; }

        [JsonProperty("upload_user_action")]
        internal string UploadUserAction { get; private set; }

        [JsonProperty("ui_config")]
        internal UIConfig UIConfig { get; private set; }

        [JsonProperty("holiday")]
        internal List<string> Holidays { get; private set; }
    }

    internal class AntiAddictionConfigResponse : BaseResponse 
    {
        [JsonProperty("data")]
        internal AntiAddictionConfigResult Result { get; private set; }
    }

    public class AntiAddictionConfig
    {
        [JsonProperty("gameId")]
        public string gameId;

        [JsonProperty("useTapLogin")]
        public bool useTapLogin;
        
        [JsonProperty("showSwitchAccount")]
        public bool showSwitchAccount = true;

        [JsonProperty("region")] public Region region = Region.China;
        
        //"g" means Displays the enumeration entry as a string value, if possible, and otherwise displays the integer value of the current instance.
        internal string regionStr => region.ToString("g").ToLower();
    }


}
