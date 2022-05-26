using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Search
{
    public  abstract  class HubspotSearchResponce
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }
    }
    public class Result
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }
    }
    public class Properties
    {
    }
}
