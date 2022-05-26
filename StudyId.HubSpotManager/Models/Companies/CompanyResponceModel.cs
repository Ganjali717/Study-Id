using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Companies
{
    public class CompanyResponceModel
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
