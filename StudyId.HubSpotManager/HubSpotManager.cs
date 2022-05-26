using System.Net;
using Envirotech.Portal.HubSpotManager;
using Envirotech.Portal.HubSpotManager.Entities.Engagements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StudyId.HubSpotManager.Models;
using StudyId.HubSpotManager.Models.Companies;
using StudyId.HubSpotManager.Models.Contacts;
using StudyId.HubSpotManager.Models.Contacts.AgentContacts;
using StudyId.HubSpotManager.Models.Contacts.StudentContacts;
using StudyId.HubSpotManager.Models.Deals;
using StudyId.HubSpotManager.Models.Engagements;
using StudyId.HubSpotManager.Models.Files;
using StudyId.HubSpotManager.Models.Search;

namespace StudyId.HubSpotManager
{
    public class HubSpotManager : WebRequestManager.WebRequestManager, IHubSpotManager
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
        private readonly IConfiguration _configuration;
        private readonly ILogger<HubSpotManager> _log;
        private readonly string _baseEndpoint = "https://api.hubapi.com";

        private readonly string _apiKey;
        private readonly string _pipelineId;
        private readonly string _pipelineStage;


        public HubSpotManager(IConfiguration configuration, ILogger<HubSpotManager> log)
        {
            _configuration = configuration;
            _log = log;
            _apiKey = _configuration["HubSpot:ApiKey"];
            _pipelineId = _configuration["HubSpot:PipelineId"];
            _pipelineStage = _configuration["HubSpot:PipelineStage"];
            if (string.IsNullOrEmpty(_apiKey)) throw new ArgumentException("HubSpot:ApiKey not found in appsettings.json file");
        }

        public HubspotResult<string> SearchCompany(string domain)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest($"{_baseEndpoint}/crm/v3/objects/companies/search?hapikey={_apiKey}", "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var query = new CompanySearchRequestModel();
                query.FilterGroups.Add(new FilterGroup() { Filters = new List<Filter>() { new Filter() { Operator = "EQ", PropertyName = "domain", Value = domain } } });

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(query);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                if (string.IsNullOrEmpty(responce))
                {
                    result.Message = $"Company with domain:'{domain}' has not found.";
                    return result;
                }
                var dbResponce = JsonConvert.DeserializeObject<CompanySearchResponceModel>(responce);
                if (dbResponce.Results.Any())
                {
                    result.Success = true;
                    result.Data = dbResponce.Results.FirstOrDefault()?.Id;
                }

            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> CreateOrUpdateCompany(CompanyRequestModel model)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest(model.IsUpdate ? $"{_baseEndpoint}/crm/v3/objects/companies/{model.Id}?hapikey={_apiKey}" : $"{_baseEndpoint}/crm/v3/objects/companies?hapikey={_apiKey}", model.IsUpdate ? "PATCH" : "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var query = new
                {
                    properties = model
                };
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(query);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<CompanyResponceModel>(responce);
                result.Success = true;
                result.Data = dbResponce.Id.ToString();


            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> AssociateCompanyWithContact(string companyId, string contactId)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest($"https://api.hubapi.com/crm/v3/objects/companies/{companyId}/associations/contact/{contactId}/2?hapikey={_apiKey}", "PUT");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<CompanyResponceModel>(responce);
                result.Success = true;
                result.Data = dbResponce.Id.ToString();


            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> SearchContact(string email)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest($"{_baseEndpoint}/crm/v3/objects/contacts/search?hapikey={_apiKey}", "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var query = new ContactSearchRequestModel();
                query.FilterGroups.Add(new FilterGroup() { Filters = new List<Filter>() { new Filter() { Operator = "EQ", PropertyName = "email", Value = email } } });

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(query);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                if (string.IsNullOrEmpty(responce))
                {
                    result.Message = $"Contact with email:'{email}' has not found.";
                    return result;
                }
                var dbResponce = JsonConvert.DeserializeObject<ContactSearchResponceModel>(responce);
                if (dbResponce.Results.Any())
                {
                    result.Success = true;
                    result.Data = dbResponce.Results.FirstOrDefault()?.Id;
                }

            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> CreateOrUpdateContact(AgentContactRequest model)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest(model.IsUpdate ? $"{_baseEndpoint}/crm/v3/objects/contacts/{model.Id}?hapikey={_apiKey}" : $"{_baseEndpoint}/crm/v3/objects/contacts?hapikey={_apiKey}", model.IsUpdate ? "PATCH" : "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var query = new
                {
                    properties = model
                };
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(query);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<AgentContactResponceModel>(responce);
                if (string.IsNullOrEmpty(responce))
                {
                    result.Message = $"Can't create contact with email:'{model.Email}'";
                    return result;
                }
                result.Success = true;
                result.Data = dbResponce.Id.ToString();
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> CreateOrUpdateContact(StudentContactRequestModel model)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest(model.IsUpdate ? $"{_baseEndpoint}/crm/v3/objects/contacts/{model.Id}?hapikey={_apiKey}" : $"{_baseEndpoint}/crm/v3/objects/contacts?hapikey={_apiKey}", model.IsUpdate ? "PATCH" : "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var query = new
                {
                    properties = model
                };
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(query);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<StudentContactResponceModel>(responce);
                if (string.IsNullOrEmpty(responce))
                {
                    result.Message = $"Can't create contact with email:'{model.Email}'";
                    return result;
                }
                result.Success = true;
                result.Data = dbResponce.Id.ToString();
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> SearchDeal(string title)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest($"{_baseEndpoint}/crm/v3/objects/deals/search?hapikey={_apiKey}", "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var query = new ContactSearchRequestModel();
                query.FilterGroups.Add(new FilterGroup() { Filters = new List<Filter>()
                {
                    new Filter() { Operator = "EQ", PropertyName = "title", Value = title }
                } });

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(query);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                if (string.IsNullOrEmpty(responce))
                {
                    result.Message = $"Deal with title:'{title}' has not found.";
                    return result;
                }
                var dbResponce = JsonConvert.DeserializeObject<ContactSearchResponceModel>(responce);
                if (dbResponce.Results.Any())
                {
                    result.Success = true;
                    result.Data = dbResponce.Results.FirstOrDefault()?.Id;
                }

            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }


        public HubspotResult<string> CreateOrUpdateDeal(DealRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Stage))
            {
                model.Stage = _pipelineStage;
            }

            if (string.IsNullOrEmpty(model.Pipeline))
            {
                model.Pipeline = _pipelineId;
            }
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest(model.IsUpdate ? $"{_baseEndpoint}/crm/v3/objects/deals/{model.Id}?hapikey={_apiKey}" : $"{_baseEndpoint}/crm/v3/objects/deals?hapikey={_apiKey}", model.IsUpdate ? "PATCH" : "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var query = new
                {
                    properties = model
                };
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(query);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<AgentContactResponceModel>(responce);
                if (string.IsNullOrEmpty(responce))
                {
                    result.Message = $"Can't create deal with title:'{model.Title}'";
                    return result;
                }
                result.Success = true;
                result.Data = dbResponce.Id.ToString();
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }



        public HubspotResult<string> AssociateDealWithContact(string dealId, string contactId)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest($"https://api.hubapi.com/crm/v3/objects/deals/{dealId}/associations/contact/{contactId}/deal_to_contact?hapikey={_apiKey}", "PUT");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<CompanyResponceModel>(responce);
                result.Success = true;
                result.Data = dbResponce.Id.ToString();


            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> AssociateDealWithCompany(string dealId, string companyId)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest($"https://api.hubapi.com/crm/v3/objects/deals/{dealId}/associations/company/{companyId}/5?hapikey={_apiKey}", "PUT");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<CompanyResponceModel>(responce);
                result.Success = true;
                result.Data = dbResponce.Id.ToString();


            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> UploadHubspotFile(string link)
        {
            var result = new HubspotResult<string>();
            try
            {
                var fileInfo = new FileInfo(link);
                var fileBody = File.ReadAllBytes(link);
                HttpClient httpClient = new HttpClient();
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(fileBody, 0, fileBody.Length), "files", fileInfo.Name);
                HttpResponseMessage response = httpClient.PostAsync($"http://api.hubapi.com/filemanager/api/v2/files?hapikey={_apiKey}", form).Result;

                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                string responce = response.Content.ReadAsStringAsync().Result;
                var dbResponce = JsonConvert.DeserializeObject<HubspotFileResponce>(responce);
                if (dbResponce != null && dbResponce.Objects.Any())
                {
                    result.Success = true;
                    result.Data = dbResponce.Objects.FirstOrDefault()?.Id.ToString();
                }
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> UploadHubspotFile(string filename, byte[] data)
        {
            var result = new HubspotResult<string>();
            try
            {
                HttpClient httpClient = new HttpClient();
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(data, 0, data.Length), "files", filename);
                HttpResponseMessage response = httpClient.PostAsync($"http://api.hubapi.com/filemanager/api/v2/files?hapikey={_apiKey}", form).Result;

                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                string responce = response.Content.ReadAsStringAsync().Result;
                var dbResponce = JsonConvert.DeserializeObject<HubspotFileResponce>(responce);
                if (dbResponce != null && dbResponce.Objects.Any())
                {
                    result.Success = true;
                    result.Data = dbResponce.Objects.FirstOrDefault()?.Id.ToString();
                }
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }

        public HubspotResult<string> CreateOrUpdateEngagement(EngagementRequestModel model, long[] contactIds = null, long[] companyIds = null, long[] dealIds = null, long[] ownerIds = null, long[] ticketIds = null, long[] attachments = null)
        {
            var result = new HubspotResult<string>();
            try
            {
                var request = GenerateBaseWebRequest(model.IsUpdate ? $"{_baseEndpoint}/engagements/v1/engagements/{model.Engagement.Id}?hapikey={_apiKey}" : $"{_baseEndpoint}/engagements/v1/engagements?hapikey={_apiKey}", model.IsUpdate ? "PATCH" : "POST");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                if (contactIds != null)
                {
                    model.Associations.ContactIds = contactIds.ToList();
                }
                if (companyIds != null)
                {
                    model.Associations.CompanyIds = companyIds.ToList();
                }
                if (dealIds != null)
                {
                    model.Associations.DealIds = dealIds.ToList();
                }
                if (ownerIds != null)
                {
                    model.Associations.OwnerIds = ownerIds.ToList();
                }
                if (ticketIds != null)
                {
                    model.Associations.TicketIds = ticketIds.ToList();
                }
                if (attachments != null)
                {
                    model.Attachments = attachments.Select(x=>new Attachment(){Id = x}).ToList();
                }
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = JsonConvert.SerializeObject(model);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var responce = GetResponceForRequest(request);
                var dbResponce = JsonConvert.DeserializeObject<EngagementResponceModel>(responce);
                if (string.IsNullOrEmpty(responce))
                {
                    result.Message = $"Can't create deal with title:'{model.Engagement.Type}'";
                    return result;
                }
                result.Success = true;
                result.Data = dbResponce.Engagement.Id.ToString();
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                _log.LogError(message);
                result.Message = message;
            }

            return result;
        }


        //public HubspotResult<string> AssociateContactWithContact(string fromContactId, string toContactId)
        //{
        //    var result = new HubspotResult<string>();
        //    try
        //    {
        //        var request = GenerateBaseWebRequest($"https://api.hubapi.com/crm/v3/objects/contacts/{fromContactId}/associations/contact/{toContactId}?hapikey={_apiKey}", "PUT");
        //        request.ContentType = "application/json";
        //        request.Accept = "application/json";
        //        var responce = GetResponceForRequest(request);
        //        var dbResponce = JsonConvert.DeserializeObject<CompanyResponceModel>(responce);
        //        result.Success = true;
        //        result.Data = dbResponce.Id.ToString();


        //    }
        //    catch (Exception e)
        //    {
        //        var message = e.GetBaseException().Message;
        //        _log.LogError(message);
        //        result.Message = message;
        //    }

        //    return result;
        //}

        public static long ToUnixTime(DateTime dateTime)
        {
            return (dateTime - UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
