using StudyId.Entities;
using StudyId.Entities.Articles;
using StudyId.Models.Dto.Admin.Articles;

namespace StudyId.Data.Managers.Interfaces
{
    public interface ICategoriesManager
    {
        /// <summary>
        /// Search category entities by query and additional properties
        /// </summary>
        /// <param name="q">Search string</param>
        /// <param name="orderBy">Order column</param>
        /// <param name="orderAsc">Order ascending</param>
        /// <param name="page">Page number</param>
        /// <param name="take">Count of records to take</param>
        /// <returns>PagedManagerResult with the accounts entities in the data + total rows count</returns>
        PagedManagerResult<IList<Category>> GetCategories(string? q, string? orderBy, bool? orderAsc, int page = 1, int take = 25);
        /// <summary>
        /// Create the new category in the db
        /// </summary>
        /// <param name="category">Category entity</param>
        /// <returns>ManagerResult with success flag and category entity in the data field</returns>
        ManagerResult<Category> Create(Category category);
        /// <summary>
        /// Update the new category in the db
        /// </summary>
        /// <param name="category">Category entity</param>
        /// <returns>ManagerResult with success flag and category entity in the data field</returns>
        ManagerResult<Category> Update(Category category);
        ManagerResult<Category> GetCategoryByKey(long routeKey);
        /// <summary>
        /// Delete category by category id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>ManagerResult with success flag</returns>
        ManagerResult Delete(Guid id);
    }
}
