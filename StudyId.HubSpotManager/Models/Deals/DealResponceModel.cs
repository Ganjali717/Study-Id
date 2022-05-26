using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Deals
{
    public class DealResponceModel
    {
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }


        [JsonProperty("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
