using StudyId.Entities;
using StudyId.Entities.Applications;
using StudyId.Entities.Courses;

namespace StudyId.Data.Managers.Interfaces
{
    /// <summary>
    /// Provide the operations with the client applications on the website
    /// </summary>
    public interface IApplicationsManager
    {
        /// <summary>
        /// Search accounts entities by query and additional properties
        /// </summary>
        /// <param name="q">Search string</param>
        /// <param name="isAustralian">isAustralian bool filter</param>
        /// <param name="course">Filter by selected course</param>
        /// <param name="from">From date filter</param>
        /// <param name="to">To date filter</param>
        /// <param name="orderBy">Order column</param>
        /// <param name="orderAsc">Order ascending</param>
        /// <param name="page">Page number</param>
        /// <param name="take">Count of records to take</param>
        /// <returns>PagedManagerResult with the accounts entities in the data + total rows count</returns>
        PagedManagerResult<IList<Application>> GetApplications(string? q, bool? isAustralian, Guid?[] course, DateTime? from, DateTime? to, string? orderBy, bool? orderAsc, Status? status, int page = 1, int take = 25);
        /// <summary>
        /// Create or update application in the db
        /// </summary>
        /// <param name="application">Application entity</param>
        /// <returns>Manager result with Success flag and updated entity in the data field</returns>
        ManagerResult<Application> CreateOrUpdate(Application application);

        /// <summary>
        /// Remove application in the db
        /// </summary>
        /// <param name="id">Application entity</param>
        /// <returns>Manager result with Success flag and removed entity in the data field</returns>
        ManagerResult RemoveApplication(Guid id);
        
        /// <summary>
        /// Search application by the application id
        /// </summary>
        /// <param name="id">Application Id</param>
        /// <returns>ManagerResult with the Application entity in the Data field</returns>
        ManagerResult<Application> GetApplication(Guid id);
        /// <summary>
        /// Load a list of available courses
        /// </summary>
        /// <returns>ManagerResult with the List of courses in the Data field</returns>
        ManagerResult<List<Course>> GetCourses();
        /// <summary>
        /// Load a list of available courses
        /// </summary>
        /// <returns>ManagerResult with the List of tasks in the Data field</returns>
        ManagerResult<List<Entities.Tasks.Task>> GetTasks();
        /// <summary>
        /// Change the status of the application
        /// </summary>
        /// <param name="id">Application id</param>
        /// <param name="status">Status value</param>
        /// <returns>ManageResult with Success flag</returns>
        ManagerResult ChangeStatus(Guid id, Status status);
    }
}
