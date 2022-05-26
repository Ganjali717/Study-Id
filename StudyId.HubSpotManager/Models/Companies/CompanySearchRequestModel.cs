using Envirotech.Portal.HubSpotManager.Entities;
using StudyId.HubSpotManager.Models.Search;

namespace StudyId.HubSpotManager.Models.Companies
{
    public class CompanySearchRequestModel:HubspotSearchRequest
    {
        public CompanySearchRequestModel()
        {
            Limit = 1;
            FilterGroups  = new List<FilterGroup>();
            Sorts = new List<string>(){"name"};
            Properties = new List<string>(){"companyId"};

        }
    }

}
