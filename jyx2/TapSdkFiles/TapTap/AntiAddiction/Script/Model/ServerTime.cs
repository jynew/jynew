using LC.Newtonsoft.Json;

namespace TapTap.AntiAddiction.Model 
{
    internal class ServerTimeResult 
    {
        /// <summary>
        /// 服务器当前时间戳
        /// </summary>
        [JsonProperty("timestamp")]
        internal long Timestamp { get; private set; }
    }

    internal class ServerTimeResponse : BaseResponse 
    {
        [JsonProperty("data")]
        internal ServerTimeResult Result { get; private set; }
    }
}
