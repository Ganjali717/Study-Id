using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Extentions;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Applications;
using StudyId.Entities.Courses;
using StudyId.Entities.Security;
using StudyId.HubSpotManager;
using StudyId.HubSpotManager.Models.Contacts.StudentContacts;
using StudyId.HubSpotManager.Models.Deals;
using Status = StudyId.Entities.Applications.Status;

namespace StudyId.Data.Managers
{
    public class ApplicationsManager : IApplicationsManager
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ApplicationsManager> _logger;
        private readonly IMapper _mapper;
        public ApplicationsManager(IServiceProvider services, ILogger<ApplicationsManager> logger, IMapper mapper)
        {
            _services = services;
            _logger = logger;
            _mapper = mapper;
        }

        public PagedManagerResult<IList<Application>> GetApplications(string? q, bool? isAustralian, Guid?[] course, DateTime? @from, DateTime? to, string? orderBy,  bool? orderAsc, Status? status, int page = 1, int take = 25)
        {
            var result = new PagedManagerResult<IList<Application>>() { Page = page, Take = take };
            var orderList = new List<EntitySorting>();
            if (string.IsNullOrEmpty(orderBy) || !orderAsc.HasValue || !IsSortAvaliable<Application>(orderBy))
            {
                orderList.Add(new EntitySorting() { Column = "Created", SortAsc = false });
                result.OrderAsc = false;
                result.OrderBy = "Created";
            }
            else
            {
                if (orderBy.Contains(","))
                {
                    orderList.AddRange(orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(key => new EntitySorting() { Column = key, SortAsc = orderAsc.Value }));
                }
                else
                {
                    orderList.Add(new EntitySorting() { Column = orderBy, SortAsc = orderAsc.Value });
                }
                result.OrderAsc = orderAsc.Value;
                result.OrderBy = orderBy;
            }
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var items = dbContext.Applications.Include(x => x.Course).OrderBy(orderList);
                if (!string.IsNullOrEmpty(q))
                {
                    var normalizedQ = q.Replace("\t", "").ToLower();
                    items = items.Where(x =>  (x.Email.ToLower().Contains(normalizedQ)) || ((x.FirstName + " " + x.LastName).ToLower().Contains(normalizedQ)));
                                          
                }

                int corseLength = course.Length;

                if (corseLength != 0)
                {
                    items = items.Where(a => course.Any(x => x == a.CourseId));
                }
                if (isAustralian.HasValue)
                {
                    items = items.Where(x => x.AustralianResident == isAustralian.Value);
                }
                if (from.HasValue)
                {
                    items = items.Where(x => x.Created >= from.Value);
                }
                if (to.HasValue)
                {
                    items = items.Where(x => x.Created <= to.Value);
                }
                if (status.HasValue)
                {
                    items = items.Where(x => x.Status == status.Value);
                }
                var skip = (page - 1) * take;
                result.Total = items.Count();
                if (result.Total == 1)
                {
                    result.Data = items.ToList();
                }
                else
                {
                    result.Data = items.Skip(skip).Take(take).ToList();
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }


        public ManagerResult RemoveApplication(Guid id)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbAccount = dbContext.Applications.FirstOrDefault(x => x.Id == id);
                if (dbAccount == null)
                {
                    result.Message = $"Application with id:{id} was not found in the database.";
                    return result;
                }
                if (dbAccount.Email == "admin@study-id.com")
                {
                    result.Message = $"You don't have access to delete this account.";
                    return result;
                }
                dbContext.Remove(dbAccount);
                dbContext.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<Application> CreateOrUpdate(Application application)
        {
            var result = new ManagerResult<Application>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                
                if (application.Id == Guid.Empty)
                {
                    application.Created = DateTimeOffset.UtcNow;
                    application.Updated = DateTimeOffset.UtcNow;
                    dbContext.Applications.Add(application);
                    dbContext.SaveChanges();
                    var dbApplication =  dbContext.Applications.Include(x=>x.Course).FirstOrDefault(x=>x.Id==application.Id);
                    if (dbApplication != null)
                    {
                        var hubSpotSync = CreateOrUpdateHubspotData(application);
                        if (hubSpotSync.Success)
                        {
                            dbApplication.HubSpotContactId = hubSpotSync.Data.contactId;
                            dbApplication.HubSpotDealId = hubSpotSync.Data.dealId;
                            dbApplication.HubSpotCompanyId = hubSpotSync.Data.companyId;
                            dbContext.SaveChanges();
                        }
                    }
                    result.Data = dbApplication;
                }
                else
                {
                    var dbApplication = dbContext.Applications.Include(x => x.Course).First(x => x.Id == application.Id);
                    dbApplication.Email = application.Email;
                    dbApplication.FirstName = application.FirstName;
                    dbApplication.LastName = application.LastName;
                    dbApplication.Phone = application.Phone;
                    dbApplication.CourseId = application.CourseId;
                    dbApplication.StartDate = application.StartDate;
                    dbApplication.Updated = DateTimeOffset.UtcNow;
                    dbApplication.AustralianResident = application.AustralianResident;
                    dbContext.SaveChanges();
                    result.Data = dbApplication;

                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        private ManagerResult<(long contactId, long dealId, long companyId)> CreateOrUpdateHubspotData(Application application)
        {
            var result = new ManagerResult<(long contactId, long dealId, long companyId)>();
            var hubSpotManager = _services.GetRequiredService<IHubSpotManager>();
            try
            {
                long contactId = 0;
                if (application.HubSpotContactId.HasValue)
                {
                    contactId  = application.HubSpotContactId.Value;
                }
                else
                {
                    var existHubSpotContact = hubSpotManager.SearchContact(application.Email);
                    contactId = existHubSpotContact.Success switch
                    {
                        true => long.Parse(existHubSpotContact.Data),
                        false => long.Parse(hubSpotManager
                            .CreateOrUpdateContact(_mapper.Map<StudentContactRequestModel>(application))
                            .Data)
                    };
                }
                long dealId = 0;
                if (application.HubSpotDealId.HasValue)
                {
                    dealId = application.HubSpotDealId.Value;
                }
                else
                {
                    var dealItem = _mapper.Map<DealRequestModel>(application);
                    dealId = long.Parse(hubSpotManager.CreateOrUpdateDeal(dealItem).Data);

                }
                var companyId = application.HubSpotCompanyId ?? 8615707972;
                hubSpotManager.AssociateCompanyWithContact(companyId.ToString(), contactId.ToString());
                hubSpotManager.AssociateDealWithContact(dealId.ToString(), contactId.ToString());
                hubSpotManager.AssociateDealWithCompany(dealId.ToString(), companyId.ToString());
                result.Data = (contactId, dealId, companyId);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
          

        }
        public ManagerResult<Application> GetApplication(Guid id)
        {
            var result = new ManagerResult<Application>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Applications.Include(x => x.Course).FirstOrDefault(x => x.Id == id);

                if (dbApplication == null)
                {
                    result.Message = $"Application with id:{id} was not found in the database.";
                    return result;
                }
                result.Data = dbApplication;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<List<Course>> GetCourses()
        {
            var result = new ManagerResult<List<Course>>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                result.Data = dbContext.Courses.ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<List<Entities.Tasks.Task>> GetTasks()
        {
            var result = new ManagerResult<List<Entities.Tasks.Task>>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                result.Data = dbContext.Tasks.ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult ChangeStatus(Guid id, Status status)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Applications.FirstOrDefault(x => x.Id == id);
                if (dbApplication == null)
                {
                    result.Message = $"Application with id:{id} was not found in the db";
                    return result;
                }

                dbApplication.Status = status;
                dbApplication.Updated = DateTimeOffset.UtcNow;
                dbContext.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        private bool IsSortAvaliable<T>(string column)
        {
            if (column.Contains('.'))
            {
                return true;
            }
            var properies = typeof(T).GetProperties();
            if (properies.Any(x =>
                    column.Split(',', StringSplitOptions.RemoveEmptyEntries).Any(o =>
                        string.Equals(o, x.Name, StringComparison.CurrentCultureIgnoreCase))))
            {
                return true;
            }
            return false;
        }

        
    }
}
