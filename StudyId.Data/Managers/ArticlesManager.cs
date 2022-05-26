using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Extentions;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Articles;

namespace StudyId.Data.Managers
{
    public class ArticlesManager : IArticlesManager
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ArticlesManager> _logger;
        private readonly ICacheManager _cacheManager;
        private const string TagsCacheKey = "articles_tags";
        private const string RelatedCachePrefix = "articles_related_";


        public ArticlesManager(IServiceProvider services,
            ILogger<ArticlesManager> logger, ICacheManager cacheManager)
        {
            _services = services;
            _logger = logger;
            _cacheManager = cacheManager;
        }

        public PagedManagerResult<IList<Article>> GetArticles(string? q, Guid? category, DateTime? @from, DateTime? to, string? orderBy, bool? orderAsc, int page = 1, int take = 25, string? tag = null)
        {
            var result = new PagedManagerResult<IList<Article>>() { Page = page, Take = take };
            var orderList = new List<EntitySorting>();
            if (string.IsNullOrEmpty(orderBy) || !orderAsc.HasValue || !IsSortAvaliable<Article>(orderBy))
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
                var items = dbContext.Articles.OrderBy(orderList);
                if (!string.IsNullOrEmpty(q))
                {
                    var normalizedQ = q.Replace("\t", "").ToLower();
                    items = items.Where(x => (x.Title.ToLower().Contains(normalizedQ)) || (x.Route.ToLower().Contains(normalizedQ)));
                }
                if (from.HasValue)
                {
                    items = items.Where(x => x.Created >= from.Value);
                }
                if (to.HasValue) 
                {
                    items = items.Where(x => x.Created <= to.Value);
                }
                if (category.HasValue)
                {
                    items = items.Where(x => x.CategoryId == category);
                }

                /*if (!string.IsNullOrEmpty(tag))
                {
                    var normalizedQ = tag.ToLower();
                    items = items.Where(x =>
                            (x.Tags.ToLower().Contains(normalizedQ)));
                }*/
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

        public ManagerResult<Article> Create(Article model)
        {
            var result = new ManagerResult<Article>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                dbContext.Add(model);
                dbContext.SaveChanges();
                result.Success = true;
                result.Data = model;
                _cacheManager.PurgeCache(TagsCacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<Article> Edit(Article model)
        {
            var result = new ManagerResult<Article>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Articles.First(x => x.Id == model.Id);
                dbApplication.CategoryId = model.CategoryId;
                dbApplication.Title = model.Title;
                dbApplication.Route = model.Route;
                dbApplication.RouteKey = model.RouteKey;
                dbApplication.PublishOn = model.PublishOn;
                dbApplication.Body = model.Body;
                dbApplication.ShortDescription = model.ShortDescription;
                dbApplication.Image = model.Image;
                dbApplication.Tags = model.Tags;
                dbApplication.ImageTags = model.ImageTags;
                dbApplication.IsPermanent = model.IsPermanent;
                dbApplication.Updated = DateTime.UtcNow;
                dbContext.SaveChanges();
                result.Data = dbApplication;
                result.Success = true;
                _cacheManager.PurgeCache(TagsCacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<List<string>> GetTags()
        {
            var result = new ManagerResult<List<string>>();
            try
            {
                var items = _cacheManager.CacheGetValueOrNull<List<string>>(TagsCacheKey);
                if (items != null)
                {
                    result.Data = items;
                    result.Success = true;
                    return result;
                }
                var configuration = _services.GetRequiredService<IConfiguration>();

                using var connection = new SqlConnection(configuration.GetConnectionString("StudyIdDbContext"));
                connection.Open();
                var sql = "SELECT DISTINCT TOP(100) value as tag FROM [dbo].[Articles] art WITH(NOLOCK) CROSS APPLY STRING_SPLIT(Tags, ',') WHERE art.Tags<>'' AND value<>''";
                result.Data = connection.Query<string>(sql).ToList();
                result.Success = true;
                _cacheManager.CacheSetValue(TagsCacheKey, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<Article> GetArticle(Guid id)
        {
            var result = new ManagerResult<Article>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Articles.Include(x => x.Category).FirstOrDefault(x => x.Id == id);

                if (dbApplication == null)
                {
                    result.Message = $"Article with id:{id} was not found in the database.";
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

        public ManagerResult<Article> GetArticle(long key)
        {
            var result = new ManagerResult<Article>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Articles.Include(x => x.Category).FirstOrDefault(x => x.RouteKey == key);

                if (dbApplication == null)
                {
                    result.Message = $"Article with key:{key} was not found in the database.";
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

        public ManagerResult<List<Article>> GetRelatedPosts(Guid id)
        {
            var result = new ManagerResult<List<Article>>();
            try
            {
                var items = _cacheManager.CacheGetValueOrNull<List<Article>>($"{RelatedCachePrefix}{id}");
                if (items != null)
                {
                    result.Data = items;
                    result.Success = true;
                    return result;
                }
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbArticle = dbContext.Articles.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (dbArticle == null)
                {
                    result.Message = $"Article with id:{id} was not found in the database.";
                    return result;
                }

                result.Data = dbContext.Articles.Include(x => x.Category)
                    .OrderByDescending(x => x.PublishOn)
                    .ThenByDescending(x => x.Created)
                    .Where(x => x.CategoryId == dbArticle.CategoryId && x.Id != id && x.Status == Status.Published).Take(3).ToList();
                result.Success = true;
                _cacheManager.CacheSetValue($"{RelatedCachePrefix}{id}", result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<List<Article>> GetLastPosts()
        {
            var result = new ManagerResult<List<Article>>();
            try
            {
                var items = _cacheManager.CacheGetValueOrNull<List<Article>>($"{RelatedCachePrefix}_top");
                if (items != null)
                {
                    result.Data = items;
                    result.Success = true;
                    return result;
                }
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                result.Data = dbContext.Articles.Include(x => x.Category)
                    .OrderByDescending(x => x.PublishOn)
                    .ThenByDescending(x => x.Created)
                    .Where(x => x.Status== Status.Published).Take(3).ToList();
                result.Success = true;
                _cacheManager.CacheSetValue($"{RelatedCachePrefix}_top", result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<Article> RemoveArticle(Guid id)
        {
            var result = new ManagerResult<Article>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbArticle = dbContext.Articles.FirstOrDefault(x => x.Id == id);
                if (dbArticle == null)
                {
                    result.Message = $"Article with id:{id} was not found in the database.";
                    return result;
                }

                if (dbArticle.IsPermanent)
                {
                    result.Message = $"Article with id:{id} is Permanent so it's using for the internal links. You can't delete it.";
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
