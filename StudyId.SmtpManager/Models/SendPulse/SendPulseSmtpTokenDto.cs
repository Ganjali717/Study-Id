using Newtonsoft.Json;

namespace StudyId.SmtpManager.Models.SendPulse
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SendPulseSmtpTokenDto
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
