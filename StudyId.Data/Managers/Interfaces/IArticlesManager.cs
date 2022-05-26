using StudyId.Entities;
using StudyId.Entities.Articles;

namespace StudyId.Data.Managers.Interfaces
{
    public interface IArticlesManager
    {
        /// <summary>
        /// Search articles entities by query and additional properties
        /// </summary>
        /// <param name="q">Search string</param>
        /// <param name="category">Filter by selected category</param>
        /// <param name="from">From date filter</param>
        /// <param name="to">To date filter</param>
        /// <param name="orderBy">Order column</param>
        /// <param name="orderAsc">Order ascending</param>
        /// <param name="page">Page number</param>
        /// <param name="take">Count of records to take</param>
        /// <returns>PagedManagerResult with the accounts entities in the data + total rows count</returns>
        PagedManagerResult<IList<Article>> GetArticles(string? q, Guid? category, DateTime? from, DateTime? to, string? orderBy, bool? orderAsc,  int page = 1, int take = 25, string? tag=null);
        ManagerResult<Article> Create(Article model);
        ManagerResult<Article> Edit(Article model);
        /// <summary>
        /// Load all existing tags in the articles
        /// </summary>
        /// <returns>ManagerResult with the list of tags</returns>
        ManagerResult<List<string>> GetTags();
        /// <summary>
        /// Search article by the id
        /// </summary>
        /// <param name="id">Article Id</param>
        /// <returns>ManagerResult with the Article entity in the Data field</returns>
        ManagerResult<Article> GetArticle(Guid id);
        /// <summary>
        /// Search article by the id
        /// </summary>
        /// <param name="key">Article Key</param>
        /// <returns>ManagerResult with the Article entity in the Data field</returns>
        ManagerResult<Article> GetArticle(long key);
        /// <summary>
        /// Get related posts for the article by the category
        /// </summary>
        /// <param name="id">Current Article Id</param>
        /// <returns></returns>
        /// 
        ManagerResult<List<Article>> GetRelatedPosts(Guid id);
        /// <summary>
        /// Get last posts for the article by the category
        /// </summary>
        /// <param name="id">Current Article Id</param>
        /// <returns></returns>
        ManagerResult<List<Article>> GetLastPosts();
        /// <summary>
        /// Search article by the id
        /// </summary>
        /// <param name="id">Article Id</param>
        /// <returns>ManagerResult with the Article entity in the Data field</returns>
        ManagerResult<Article> RemoveArticle(Guid id);

        //ManagerResult<List<categ>> UpdateAsync(Article model);
    }
}
