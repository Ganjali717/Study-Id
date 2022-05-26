using System.Net.Mail;
using Newtonsoft.Json;
namespace StudyId.SmtpManager.Models.SendPulse
{
    public class SendPulseMailDto
    {
        [JsonProperty("email")]
        public SendPulseMailBody Email { get; set; }
    }
    public class SendPulseMailBody
    {
        public SendPulseMailBody()
        {
            Attachments = new List<string>();
            AttachmentsBinary = new Dictionary<string, string>();
        }
        [JsonProperty("html")]
        public string Html { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("from")]
        public SendPulseMailAddress From { get; set; }
        [JsonProperty("to")]
        public List<SendPulseMailAddress> To { get; set; }
        [JsonProperty("attachments")]
        public List<string> Attachments { get; set; }
        [JsonProperty("attachments_binary")]
        public Dictionary<string, string> AttachmentsBinary { get; set; }
    }
    public class SendPulseMailAddress
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
