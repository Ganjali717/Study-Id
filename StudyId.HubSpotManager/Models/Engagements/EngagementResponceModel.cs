using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using StudyId.HubSpotManager.Models.Engagements;

namespace Envirotech.Portal.HubSpotManager.Entities.Engagements
{
    public class EngagementResponceModel
    {
        [JsonProperty("engagement")]
        public Engagement Engagement { get; set; }

        [JsonProperty("associations")]
        public Associations Associations { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}
