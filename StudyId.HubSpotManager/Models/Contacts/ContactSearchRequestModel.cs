using Envirotech.Portal.HubSpotManager.Entities;
using StudyId.HubSpotManager.Models.Search;

namespace StudyId.HubSpotManager.Models.Contacts
{
    public class ContactSearchRequestModel:HubspotSearchRequest
    {

        public ContactSearchRequestModel()
        {
            Limit = 1;
            FilterGroups  = new List<FilterGroup>();
            Sorts = new List<string>(){"name"};
            Properties = new List<string>(){"contactId"};
        }
    }

}
