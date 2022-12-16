using LC.Newtonsoft.Json;

namespace TapTap.AntiAddiction.Model 
{
    public class PayableResult 
    {
        [JsonProperty("code")]
        public int Code { get; internal set; }

        [JsonProperty("status")]
        public bool Status { get; internal set; }

        [JsonProperty("title")]
        public string Title { get; internal set; }

        [JsonProperty("description")]
        public string Content { get; internal set; }
    }

    internal class PayableResponse : BaseResponse 
    {
        [JsonProperty("data")]
        internal PayableResult Result { get; private set; }
    }
}
