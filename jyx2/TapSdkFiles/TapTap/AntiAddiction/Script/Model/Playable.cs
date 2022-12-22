using LC.Newtonsoft.Json;
using TapTap.AntiAddiction.Model;

namespace TapTap.AntiAddiction 
{
    public class PlayableResult 
    {
        internal static readonly int ADULT = 0;
        internal static readonly int NIGHT_STRICT = 1;
        internal static readonly int TIME_LIMIT = 2;
        private static readonly int MAX_VIETNAM_REMAIN_TIME = 180;
        private static readonly int MAX_CHINA_REMAIN_TIME = 60;
        
        /// <summary>
        /// 单日游戏最大剩余时间(分钟)
        /// </summary>
        internal static int MaxRemainTime
        {
            get
            {
                if (TapTapAntiAddictionManager.AntiAddictionConfig.region == Region.Vietnam)
                    return MAX_VIETNAM_REMAIN_TIME;
                return MAX_CHINA_REMAIN_TIME;
            }
        }
        /// <summary>
        /// 限制类型，
        /// 0: 成年人，无限制
        /// 1: 宵禁
        /// 2: 时长
        /// </summary>
        [JsonProperty("restrict_type")]
        public int RestrictType { get; internal set; }

        /// <summary>
        /// 是否可玩
        /// </summary>
        [JsonProperty("can_play")]
        public bool CanPlay { get; internal set; }

        /// <summary>
        /// 剩余时长，用于 UI 展示
        /// </summary>
        [JsonProperty("remain_time")]
        public int RemainTime { get; internal set; }
        /// <summary>
        /// 游玩时间
        /// </summary>
        [JsonProperty("cost_time")]
        public int CostTime { get; internal set; }

        /// <summary>
        /// 标题，用于 UI 展示
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; internal set; }

        /// <summary>
        /// 内容，用于 UI 展示
        /// </summary>
        [JsonProperty("description")]
        public string Content { get; internal set; }

        /// <summary>
        /// 判断是否是成年人
        /// </summary>
        public bool IsAdult => RestrictType == ADULT;
    }

    internal class PlayableResponse : BaseResponse 
    {
        [JsonProperty("data")]
        public PlayableResult Result;
    }
}