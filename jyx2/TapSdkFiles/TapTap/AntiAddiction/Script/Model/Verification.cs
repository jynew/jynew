using LC.Newtonsoft.Json;
using TapTap.AntiAddiction;

namespace TapTap.AntiAddiction.Model 
{
    public class VerificationResult 
    {
        [JsonProperty("age_limit")]
        internal int AgeLimit { get; private set; }

        [JsonProperty("status")]
        internal int Status { get; private set; }

        [JsonProperty("anti_addiction_token")] 
        internal string AntiAddictionToken { get; private set; }

        [JsonProperty("has_auth_record")] 
        internal bool HasAuthRecord { get; private set; }
        
        internal VerificationResult() { }

        internal VerificationResult(VerificationResult other) 
        {
            AgeLimit = other.AgeLimit;
            Status = other.Status;
            AntiAddictionToken = other.AntiAddictionToken;
            HasAuthRecord = other.HasAuthRecord;
        }
    }

    internal class VerificationResponse 
    {
        [JsonProperty("type")]
        internal string Type { get; private set; }

        [JsonProperty("message")]
        internal VerificationResult Result { get; private set; }
    }

    internal class ServerVerificationResponse : BaseResponse 
    {
        [JsonProperty("data")]
        internal VerificationResult Result;
        [JsonProperty("code")]
        internal string Code { get; private set; }        
        [JsonProperty("msg")]
        internal string Message { get; private set; }
    }

    internal class LocalVerification : VerificationResult
    {
        [JsonProperty("user_id")]
        internal string UserId { get; set; }

        [JsonProperty("region")] internal Region Region = Region.China;

        internal LocalVerification() { }

        internal LocalVerification(VerificationResult obj) : base(obj) { }

        /// <summary>
        /// 是否已认证
        /// </summary>
        internal bool IsVerified => Status == 0;        
        
        /// <summary>
        /// 是否在认证中
        /// /// </summary>
        internal bool IsVerifing => Status == 1;

        /// <summary>
        /// 是否是成人
        /// </summary>
        internal bool IsAdult => AgeLimit == 18;
    }
}
