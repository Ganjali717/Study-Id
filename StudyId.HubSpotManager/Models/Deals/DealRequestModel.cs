using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Deals
{
    public class DealRequestModel
    {
        [JsonIgnore]
        public string Id { get; set; }

        [JsonProperty("dealname")]
        public string Title { get; set; }
        [JsonProperty("dealstage")]
        //Admission - Incomplete:(Live)1870259,(Stage)3974699
        public string Stage { get; set; }
        [JsonProperty("preferred_qualification")]
        public string PrefferedQualification { get; set; }
        [JsonIgnore]
        public DateTime? PrefferedStartDate { get; set; }
        
        [JsonProperty("preffered_start_date")]
        public long? PrefferedStartDateValue => PrefferedStartDate.HasValue ? HubSpotManager.ToUnixTime(PrefferedStartDate.Value) : null;

        //[JsonProperty("hubspot_owner_id")]
        //public string Owner { get; set; }
        /// <summary>
        /// newbusiness, existingbusiness
        /// </summary>
        [JsonProperty("dealtype")]
        public string DealType => "newbusiness";

        [JsonProperty("pipeline")] 
        public string Pipeline { get; set; }
        [JsonProperty("portal_link")]
        public string PortalLink { get; set; }
        [JsonProperty("australia_resident")]
        public bool AustraliaResident { get; set; }
        [JsonIgnore]
        public bool IsUpdate => !string.IsNullOrEmpty(Id);

    }

}
