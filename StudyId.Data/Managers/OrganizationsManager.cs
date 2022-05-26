using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Extentions;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Courses;
using StudyId.Entities.Organizations;

namespace StudyId.Data.Managers
{
    public class OrganizationsManager:IOrganizationsManager
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<OrganizationsManager> _logger;

        public OrganizationsManager(IServiceProvider services, ILogger<OrganizationsManager> logger)
        {
            _services = services;
            _logger = logger;
        }
        public PagedManagerResult<IList<Organization>> GetOrganizations(string? q, DateTime? from, DateTime? to, string? orderBy, bool? orderAsc, Status? status, string offset, int page = 1, int take = 25)
        {
            var result = new PagedManagerResult<IList<Organization>>() { Page = page, Take = take };
            var orderList = new List<EntitySorting>();
            if (string.IsNullOrEmpty(orderBy) || !orderAsc.HasValue || !IsSortAvaliable<Organization>(orderBy))
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
                var items = dbContext.Organizations.Include(x => x.Courses).Include(x=>x.Persons).Include(x=> x.Documents).OrderBy(orderList);
                
                if (!string.IsNullOrEmpty(q))
                {
                    var normalizedQ = q.ToLower();
                    items = items.Where(x => (x.Email.ToLower().Contains(normalizedQ)) ||  (x.Title.ToLower().Contains(normalizedQ)));
                }
                
                if (from.HasValue)
                {
                    items = items.Where(x => offset == "created"?(x.Created >= from.Value):(x.Updated >= from.Value));
                }
                if (to.HasValue)
                {
                    items = items.Where(x => offset == "created"?(x.Created <= to.Value):(x.Updated <= to.Value));
                }
                if (status.HasValue)
                {
                    items = items.Where(x => x.Status == status.Value);
                }
                var skip = (page - 1) * take;
                result.Total = items.Count();
                result.Data = items.Skip(skip).Take(take).ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }


        public ManagerResult<Organization> CreateOrUpdate(Organization organization)
        {
            var result = new ManagerResult<Organization>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                if (organization.Id == Guid.Empty)
                {
                    organization.Created = DateTimeOffset.UtcNow;
                    organization.Updated = DateTimeOffset.UtcNow;
                    dbContext.Organizations.Add(organization);
                    dbContext.SaveChanges();

                    result.Data = organization;
                }
                else
                {
                    var dbApplication = dbContext.Organizations.Include(x => x.Courses).First(x => x.Id == organization.Id);
                   
                    dbApplication.Title = organization.Title;
                    dbApplication.TaxNumber = organization.TaxNumber;
                    dbApplication.Address = organization.Address;
                    dbApplication.Phone = organization.Phone;
                    dbApplication.Email = organization.Email;
                    dbApplication.StartDate = organization.StartDate;
                    dbApplication.Courses = organization.Courses;
                    dbApplication.Persons = organization.Persons;
                    dbApplication.Updated = DateTimeOffset.UtcNow;
                    dbContext.SaveChanges();
                    var removedItems = dbApplication.Courses.Where(x => organization.Courses.All(c => c.CourseId != x.CourseId)).ToList();
                    var removedPersonItems = dbApplication.Persons.Where(x => organization.Persons.All(c => c.Id != x.Id)).ToList();
                    var insertedItems = organization.Courses.Where(x => dbApplication.Courses.All(c => c.CourseId != x.CourseId)).ToList();
                    var insertedPersonItems = organization.Persons.Where(x => dbApplication.Persons.All(c => c.Id != x.Id)).ToList();

                    if (removedItems.Any() || removedPersonItems.Any())
                    {
                        dbContext.OrganizationsCourses.RemoveRange(removedItems);
                        dbContext.OrganizationPersons.RemoveRange(removedPersonItems);
                    }
                    if (insertedItems.Any() || removedPersonItems.Any())
                    {
                        dbContext.OrganizationsCourses.AddRange(insertedItems);
                        dbContext.OrganizationPersons.AddRange(insertedPersonItems);
                    }
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

        public ManagerResult<List<OrganizationCourse>> GetOrganizationCourses()
        {
            var result = new ManagerResult<List<OrganizationCourse>>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                result.Data = dbContext.OrganizationsCourses.ToList();
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
                var dbApplication = dbContext.Organizations.FirstOrDefault(x => x.Id == id);
                if (dbApplication == null)
                {
                    result.Message = $"Task with id:{id} was not found in the db";
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

        public ManagerResult<Organization> RemoveOrganization(Guid id)
        {
            var result = new ManagerResult<Organization>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbArticle = dbContext.Organizations.FirstOrDefault(x => x.Id == id);
                if (dbArticle == null)
                {
                    result.Message = $"Organization with id:{id} was not found in the database.";
                    return result;
                }
                
                dbContext.Remove(dbArticle);
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

        public ManagerResult<Organization> GetOrganization(Guid id)
        {
            var result = new ManagerResult<Organization>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Organizations.Include(x => x.Courses).Include(x=> x.Persons).Include(x=> x.Documents).FirstOrDefault(x => x.Id == id);

                if (dbApplication == null)
                {
                    result.Message = $"Organization with id:{id} was not found in the database.";
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
    }
}
