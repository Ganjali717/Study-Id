using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Extentions;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Applications;
using StudyId.Entities.Courses;
using StudyId.Entities.Organizations;
using Status = StudyId.Entities.Tasks.Status;

namespace StudyId.Data.Managers
{
    public class TasksManager:ITasksManager
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TasksManager> _logger;

        public TasksManager(IServiceProvider services, ILogger<TasksManager> logger)
        {
            _services = services;
            _logger = logger;
        }
        public PagedManagerResult<IList<StudyId.Entities.Tasks.Task>> GetTasks(string? q, DateTime? from, DateTime? to, string? orderBy, bool? orderAsc, Status? status, string offset, int page = 1, int take = 25)
        {
            var result = new PagedManagerResult<IList<StudyId.Entities.Tasks.Task>>() { Page = page, Take = take };
            var orderList = new List<EntitySorting>();
            if (string.IsNullOrEmpty(orderBy) || !orderAsc.HasValue || !IsSortAvaliable<StudyId.Entities.Tasks.Task>(orderBy))
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
                var items = dbContext.Tasks.Include(x => x.Applications).ThenInclude(x=> x.Course).OrderBy(orderList);
                if (!string.IsNullOrEmpty(q))
                {
                    var normalizedQ = q.ToLower();
                    items = items.Where(x => ((x.Applications.FirstName + " " + x.Applications.LastName).ToLower().Contains(normalizedQ)) 
                        || (x.Title.ToLower().Contains(normalizedQ)));
                }

                if (from.HasValue)
                {
                    items = items.Where(x => offset == "created" ? (x.Created >= from.Value) : (x.Updated >= from.Value));
                }
                if (to.HasValue)
                {
                    items = items.Where(x => offset == "created" ? (x.Created <= to.Value) : (x.Updated <= to.Value));
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

        public ManagerResult<List<Application>> GetApplications()
        {
            var result = new ManagerResult<List<Application>>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                result.Data = dbContext.Applications.Include(x=> x.Course).ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        

        public ManagerResult<Entities.Tasks.Task> CreateOrUpdate(Entities.Tasks.Task task)
        {
            var result = new ManagerResult<Entities.Tasks.Task>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                if (task.Id == Guid.Empty)
                {
                    task.Created = DateTimeOffset.UtcNow;
                    task.Updated = DateTimeOffset.UtcNow;
                    dbContext.Tasks.Add(task);
                    dbContext.SaveChanges();

                    result.Data = task;
                }
                else
                {
                    var dbApplication = dbContext.Tasks.Include(x => x.Applications).ThenInclude(x=>x.Course).First(x => x.Id == task.Id);

                    dbApplication.Title = task.Title;
                    dbApplication.TaskPhone = task.TaskPhone;
                    dbApplication.TaskEmail = task.TaskEmail;
                    dbApplication.Status = task.Status;
                    dbApplication.Description = task.Description;
                    dbApplication.DueDate = task.DueDate;
                    dbApplication.Applications = task.Applications;
                    dbApplication.ApplicationId = task.ApplicationId;
                    dbApplication.Updated = DateTimeOffset.UtcNow;
                    dbApplication.Created = DateTimeOffset.UtcNow;
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

        public ManagerResult ChangeStatus(Guid id, Status status)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Tasks.FirstOrDefault(x => x.Id == id);
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

        public ManagerResult<Entities.Tasks.Task> GetTask(Guid id)
        {
            var result = new ManagerResult<Entities.Tasks.Task>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Tasks.Include(x => x.Applications).FirstOrDefault(x => x.Id == id);

                if (dbApplication == null)
                {
                    result.Message = $"Task with id:{id} was not found in the database.";
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

        public ManagerResult<Entities.Tasks.Task> RemoveTask(Guid id)
        {
            var result = new ManagerResult<Entities.Tasks.Task>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbArticle = dbContext.Tasks.FirstOrDefault(x => x.Id == id);
                if (dbArticle == null)
                {
                    result.Message = $"Task with id:{id} was not found in the database.";
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
