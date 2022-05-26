using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyId.Entities;
using StudyId.Entities.Courses;
using StudyId.Entities.Organizations;

namespace StudyId.Data.Managers.Interfaces
{
    public interface IOrganizationsManager
    {
        /// <summary>
        /// Search accounts entities by query and additional properties
        /// </summary>
        /// <param name="q">Search string</param>
        /// <param name="from">From date filter</param>
        /// <param name="to">To date filter</param>
        /// <param name="orderBy">Order column</param>
        /// <param name="orderAsc">Order ascending</param>
        /// <param name="page">Page number</param>
        /// <param name="take">Count of records to take</param>
        /// <returns>PagedManagerResult with the accounts entities in the data + total rows count</returns>
        PagedManagerResult<IList<Organization>> GetOrganizations(string? q, DateTime? from, DateTime? to, string? orderBy, bool? orderAsc, Status? status, string offset, int page = 1, int take = 25);
        /// <summary>
        /// Create or update application in the db
        /// </summary>
        /// <param name="organization">Application entity</param>
        /// <returns>Manager result with Success flag and updated entity in the data field</returns>
        ManagerResult<Organization> CreateOrUpdate(Organization organization);
        /// <summary>
        /// Load a list of available courses
        /// </summary>
        /// <returns>ManagerResult with the List of courses in the Data field</returns>
        ManagerResult<List<Course>> GetCourses();
        /// <summary>
        /// Load a list of available courses
        /// </summary>
        /// <returns>ManagerResult with the List of courses in the Data field</returns>
        ManagerResult<List<OrganizationCourse>> GetOrganizationCourses();
        /// <summary>
        /// Change the status of the application
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <param name="status">Status value</param>
        /// <returns>ManageResult with Success flag</returns>
        ManagerResult ChangeStatus(Guid id, Status status);
        /// <summary>
        /// Search article by the id
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns>ManagerResult with the Organization entity in the Data field</returns>
        ManagerResult<Organization> RemoveOrganization(Guid id);
        /// <summary>
        /// Search application by the application id
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns>ManagerResult with the Task entity in the Data field</returns>
        ManagerResult<Organization> GetOrganization(Guid id);
    }
}
