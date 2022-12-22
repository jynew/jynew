using LC.Newtonsoft.Json;

namespace TapTap.AntiAddiction.Model 
{
    internal class ErrorResult 
    {
        [JsonProperty("code")]
        internal int Code { get; private set; }

        [JsonProperty("error")]
        internal string Error { get; private set; }

        [JsonProperty("error_description")]
        internal string Description { get; private set; }

        [JsonProperty("msg")]
        internal string Message { get; private set; }
    }

    internal class ErrorResponse : BaseResponse 
    {
        [JsonProperty("data")]
        internal ErrorResult Result { get; private set; }
    }
}
