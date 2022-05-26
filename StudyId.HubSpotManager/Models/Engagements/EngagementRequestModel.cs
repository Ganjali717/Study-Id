using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Engagements
{
    public class EngagementRequestModel
    {
        public EngagementRequestModel()
        {
            Engagement  = new Engagement();
            Associations = new Associations();
            Attachments = new List<Attachment>();
            Metadata = new Metadata();
            Engagement.Timestamp = HubSpotManager.ToUnixTime(DateTime.Now);
        }
        [JsonProperty("engagement")]
        public Engagement Engagement { get; set; }

        [JsonProperty("associations")]
        public Associations Associations { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
        [JsonIgnore]
        public bool IsUpdate => Engagement?.Id!=null;
    }

    public class Associations
    {
        [JsonProperty("contactIds")]
        public List<long> ContactIds { get; set; }

        [JsonProperty("companyIds")]
        public List<long> CompanyIds { get; set; }

        [JsonProperty("dealIds")]
        public List<long> DealIds { get; set; }

        [JsonProperty("ownerIds")]
        public List<long> OwnerIds { get; set; }

        [JsonProperty("ticketIds")]
        public List<long> TicketIds { get; set; }
    }

    public class Attachment
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public class Engagement
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("ownerId")]
        public long OwnerId { get; set; }

        [JsonProperty("type")]
        ///EMAIL, CALL, MEETING, TASK, NOTE
        public string Type { get; set; }

        [JsonProperty("timestamp")]
        public long? Timestamp { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
