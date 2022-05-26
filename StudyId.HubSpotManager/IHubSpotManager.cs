using StudyId.HubSpotManager.Models;
using StudyId.HubSpotManager.Models.Companies;
using StudyId.HubSpotManager.Models.Contacts.AgentContacts;
using StudyId.HubSpotManager.Models.Contacts.StudentContacts;
using StudyId.HubSpotManager.Models.Deals;
using StudyId.HubSpotManager.Models.Engagements;

namespace StudyId.HubSpotManager
{
    public interface IHubSpotManager
    {
        /// <summary>
        /// Search hubspot company by company domain
        /// </summary>
        /// <param name="domain">Company domain without https:// and http:// prefix</param>
        /// <returns>HubspotResult with companyId in Data field if Success</returns>
        HubspotResult<string> SearchCompany(string domain);
        /// <summary>
        /// Create new company in HubSpot
        /// </summary>
        /// <param name="model">Company model</param>
        /// <returns></returns>
        HubspotResult<string> CreateOrUpdateCompany(CompanyRequestModel model);
        /// <summary>
        /// Create link beetwen company and contact inside hubspot
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        HubspotResult<string> AssociateCompanyWithContact(string companyId, string contactId);
        /// <summary>
        /// Search hubspot contact by contact email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        HubspotResult<string> SearchContact(string email);
        /// <summary>
        /// Create hubspot contact for Agent company
        /// </summary>
        /// <param name="request">Contact Model</param>
        /// <returns></returns>
        HubspotResult<string> CreateOrUpdateContact(AgentContactRequest request);
        /// <summary>
        /// Create hubspot contact for Student
        /// </summary>
        /// <param name="request">Contact Model</param>
        /// <returns></returns>
        HubspotResult<string> CreateOrUpdateContact(StudentContactRequestModel request);
        ///// <summary>
        ///// Create link beetwen contact and contact inside hubspot
        ///// </summary>
        ///// <returns></returns>
        //HubspotResult<string> AssociateContactWithContact(string fromContactId, string toContactId);

        /// <summary>
        /// Search hubspot deal by contact title
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        HubspotResult<string> SearchDeal(string title);
        /// <summary>
        /// Create hubspot deal 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        HubspotResult<string> CreateOrUpdateDeal(DealRequestModel request);
        /// <summary>
        /// Create link beetween company and contact inside hubspot
        /// </summary>
        /// <param name="dealId"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        HubspotResult<string> AssociateDealWithContact(string dealId, string contactId);
        /// <summary>
        /// Create link beetween company and contact inside hubspot
        /// </summary>
        /// <param name="dealId">Deal Id</param>
        /// <param name="companyId">Company Id</param>
        /// <returns></returns>
        HubspotResult<string> AssociateDealWithCompany(string dealId, string companyId);

        HubspotResult<string> UploadHubspotFile(string link);
        HubspotResult<string> UploadHubspotFile(string filename,byte[] data);
        HubspotResult<string> CreateOrUpdateEngagement(EngagementRequestModel request, long[] contactIds= null, long[] companyIds= null, long[]dealIds= null, long[]ownerIds = null, long[] ticketIds=null, long[] attachments=null);

    }
}
