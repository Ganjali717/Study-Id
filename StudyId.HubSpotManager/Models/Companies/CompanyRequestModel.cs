using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Companies
{
    public class CompanyRequestModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("address")]
        public string StreetAddress { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("zip")]
        public string PostalCode { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("hubspot_owner_id")]
        public string Owner { get; set; }

        [JsonProperty("type")]
        public string Type => "PARTNER";
        [JsonProperty("numberofemployees")]
        public long NumberOfEmployees { get; set; }
        [JsonIgnore]
        public bool IsUpdate => !string.IsNullOrEmpty(Id);
    }
}
