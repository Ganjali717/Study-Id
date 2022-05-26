using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Search
{
    public abstract class HubspotSearchRequest
    {
        [JsonProperty("filterGroups")]
        public List<FilterGroup> FilterGroups { get; set; }

        [JsonProperty("sorts")]
        public List<string> Sorts { get; set; }

        [JsonProperty("properties")]
        public List<string> Properties { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("after")]
        public long After { get; set; }
    }

    public class FilterGroup
    {
        [JsonProperty("filters")]
        public List<Filter> Filters { get; set; }
    }

    public class Filter
    {
        [JsonProperty("propertyName")]
        public string PropertyName { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
