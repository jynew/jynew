using LC.Newtonsoft.Json;

namespace TapTap.AntiAddiction.Model 
{
    internal class BaseResponse 
    {
        [JsonProperty("success")]
        internal bool Success { get; private set; }
    }
}
