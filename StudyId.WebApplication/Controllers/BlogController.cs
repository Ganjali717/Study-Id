using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Extentions;
using StudyId.Models.Dto.Admin.Articles;
using StudyId.Models.Dto.Blog;

namespace StudyId.WebApplication.Controllers
{
    [Route("/news-and-events")]
    public class BlogController : Controller
    {
        private readonly IArticlesManager _articlesManager;
        private readonly IMapper _mapper;
        private readonly ICategoriesManager _categoriesManager;

        public BlogController(IArticlesManager articlesManager, IMapper mapper, ICategoriesManager categoriesManager)
        {
            _articlesManager = articlesManager;
            _mapper = mapper;
            _categoriesManager = categoriesManager;
        }

        [HttpGet]
        [Route("/news-and-events/")]
        [Route("/news-and-events/c/{category}/")]
        [Route("/news-and-events/tag/{tag}/")]
        [ResponseCache(VaryByQueryKeys = new[] { "q","tag","page","category" }, Duration = 300)]
        public IActionResult Index(string? q=null, string? tag=null, int? page=null, string? category=null)
        {
            var model = new BlogSearchDto() {Q=q, Page = page ?? 1, Take = 5, Tag = tag};
            if (!string.IsNullOrEmpty(category))
            {
                var categoryResult = _categoriesManager.GetCategoryByKey(category.GetIntHash());
                if (categoryResult.Success)
                {
                    model.CategoryId = categoryResult.Data.Id;
                }
            }
            var items = _articlesManager.GetArticles(model.Q, model.CategoryId, null, null, null, null, model.Page, model.Take, model.Tag);
            model.Items = _mapper.Map<PagedManagerResult<IList<ArticleDto>>>(items);
            model.Tags = _articlesManager.GetTags().Data;
            model.Categories = _categoriesManager.GetCategories(null, null, null, 1, 10).Data;
            //if (!string.IsNullOrEmpty(q) ||!string.IsNullOrEmpty(tag) || page.HasValue || !string.IsNullOrEmpty(category) )
            //{
            //    model.NoIndex = true;
            //    ViewData["NoIndex"] = "noindex";
            //}
            return View(model);
        }
        
        [HttpGet]
        [Route("/news-and-events/{route}")]
        [ResponseCache(VaryByQueryKeys = new[] { "route" }, Duration = 300)]
        public IActionResult View(string route)
        {
            var itemResult = _articlesManager.GetArticle(route.GetIntHash());
            if (!itemResult.Success)
            {
                return RedirectToAction("Index");
            }
            var model = new BlogArticleDto();
            model.Article = _mapper.Map<ArticleDto>(itemResult.Data);
            model.Tags = _articlesManager.GetTags().Data;
            model.Categories = _categoriesManager.GetCategories(null, null, null, 1, 10).Data;
            model.RelatedArticles = _mapper.Map<List<ArticleDto>>(_articlesManager.GetRelatedPosts(model.Article.Id ?? Guid.Empty).Data);
            return View(model);
        }
    }
}
