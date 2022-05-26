using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Extentions;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Articles;

namespace StudyId.Data.Managers
{
    public class CategoriesManager : ICategoriesManager
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<CategoriesManager> _logger;
        private const string CategoriesCachePrefix = "categories";
        public CategoriesManager(IServiceProvider services,
            ILogger<CategoriesManager> logger)
        {
            _services = services;
            _logger = logger;
        }

        public PagedManagerResult<IList<Category>> GetCategories(string? q, string? orderBy, bool? orderAsc, int page = 1, int take = 25)
        {
            var result = new PagedManagerResult<IList<Category>>() { Page = page, Take = take };
            var orderList = new List<EntitySorting>();
            if (string.IsNullOrEmpty(orderBy) || !orderAsc.HasValue || !typeof(Category).GetProperties().Any(x => orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries).Any(o => string.Equals(o, x.Name, StringComparison.CurrentCultureIgnoreCase))))
            {
                orderList.Add(new EntitySorting() { Column = "Title", SortAsc = false });
                result.OrderAsc = false;
                result.OrderBy = "Title";
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
                var items = dbContext.Categories.OrderBy(orderList);
                if (!string.IsNullOrEmpty(q))
                {
                    var normalizedQ = q.ToLower();
                    items = items.Where(x => x.Title != null && x.Title.ToLower().Contains(normalizedQ));
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

        public ManagerResult<Category> Create(Category category)
        {
            var result = new ManagerResult<Category>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                if (dbContext.Categories.Any(x => x.Title.ToLower() == category.Title.ToLower()))
                {
                    result.Message = $"There is already exist category in the db with name:{category.Title}";
                    return result;
                }
                dbContext.Categories.Add(category);
                dbContext.SaveChanges();
                result.Data = category;
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }

            return result;
        }

        public ManagerResult<Category> Update(Category category)
        {
            var result = new ManagerResult<Category>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                if (dbContext.Categories.Any(x => x.Title.ToLower() == category.Title.ToLower() && x.Id!=category.Id))
                {
                    result.Message = $"There is already exist category in the db with name:{category.Title}";
                    return result;
                }

                var dbCategory = dbContext.Categories.FirstOrDefault(x => x.Id == category.Id);
                if (dbCategory == null)
                {
                    result.Message = $"Category with id:{category.Id} was not found.";
                    return result;
                }
                dbCategory.Title = category.Title;
                dbCategory.Route = category.Route;
                dbCategory.RouteKey = category.RouteKey;
                dbContext.SaveChanges();
                result.Data = dbCategory;
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }

            return result;
        }

        public ManagerResult<Category> GetCategoryByKey(long routeKey)
        {
            var result = new ManagerResult<Category>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbCategory = dbContext.Categories.FirstOrDefault(x => x.RouteKey == routeKey);

                if (dbCategory == null)
                {
                    result.Message = $"Category with key:{routeKey} was not found in the database.";
                    return result;
                }
                result.Data = dbCategory;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult Delete(Guid id)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbCategory = dbContext.Categories.FirstOrDefault(x => x.Id == id);
                if (dbCategory == null)
                {
                    result.Message = $"Category with id:{id} was not found.";
                    return result;
                }

                dbContext.Categories.Remove(dbCategory);
                dbContext.SaveChanges();
                result.Success = true;
                return result;
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
