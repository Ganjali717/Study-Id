using Newtonsoft.Json;

namespace StudyId.HubSpotManager.Models.Contacts.StudentContacts
{
    public class StudentContactResponceModel
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
