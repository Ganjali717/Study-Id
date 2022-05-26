using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Contacts.AgentContacts
{
    public class AgentContactResponceModel
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
