using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Contacts.StudentContacts
{
    public class StudentContactRequestModel
    {
        [JsonIgnore]
        public string Id { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("mobilephone")]
        public string Mobile { get; set; }
        [JsonProperty("lifecyclestage")]
        //Avaliable values:lead,subscriber,salesqualifiedlead,opportunity,customer,other
        public string LifecycleStage => "lead";
        [JsonProperty("hs_lead_status")]
        //Avaliable values:New Lead,Contacted - Emailed,Contacted - Texted,
        public string LeadStatus => "New Lead";
        [JsonIgnore]
        public bool IsUpdate => !string.IsNullOrEmpty(Id);
    }
}
