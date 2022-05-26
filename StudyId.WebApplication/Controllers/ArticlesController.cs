using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities.Articles;
using StudyId.Models.Dto.Admin.Articles;
using System.Net;
using StudyId.Entities;
using StudyId.Models.Dto;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace StudyId.WebApplication.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ArticlesController : Controller
    {
        private readonly IArticlesManager _articlesManager;
        private readonly ICategoriesManager _categoriesManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArticlesController(IArticlesManager articlesManager,
            ICategoriesManager categoriesManager,
            IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _articlesManager = articlesManager;
            _categoriesManager = categoriesManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]
        public IActionResult Search([FromBody] ArticlesSearchDto model)
        {
            var managerResult = _articlesManager.GetArticles(model.Q, model.CategoryValue, model.FromValue, model.ToValue, model.OrderBy, model.OrderAsc, model.Page, model.Take, model.Q);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            var dtoResult = _mapper.Map<PagedManagerResult<IList<ArticleDto>>>(managerResult);
            return Json(dtoResult);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Edit");
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public IActionResult Create([FromBody] ArticleDto model)
        {
            var serverPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "articles", "dynamic");
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
            if (!string.IsNullOrEmpty(model.Image)) 
            {
                var decodeResult = Base64ToImage(model.Image);
                var mainPath = Path.Combine(serverPath, $"{model.Route}.webp");
                decodeResult.Item1.SaveAsWebp(mainPath);
                model.Image = Url.Content($"/img/articles/dynamic/{model.Route}.webp");
            }
            var articleModel = _mapper.Map<Article>(model);
            var managerResult = _articlesManager.Create(articleModel);

            if (managerResult.Success) return Json(managerResult);

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 60)]
        public IActionResult Dictionaries()
        {
            var categories = _categoriesManager.GetCategories(null, null, null, 1, Int32.MaxValue).Data.Select(x => new DictionaryDto() { Title = x.Title, Value = x.Id.ToString() });
            var tags = _articlesManager.GetTags().Data.Select(x => new { key = x, value = x });
            var statuses = Enum.GetValues<Status>().Select(val => new DictionaryDto() {Title = val.ToString(), Value = val.ToString()}).ToList();
            return Json(new { categories, tags,statuses });
        }

        [HttpGet]
        public IActionResult Edit([FromRoute] Guid id)
        {
            var managerResult = _articlesManager.GetArticle(id);
            if (!managerResult.Success)
            {
                return RedirectToAction("Create");
            }
            var model = _mapper.Map<ArticleDto>(managerResult.Data);
            return View(model);
        }

        [HttpPost]
        public IActionResult Remove([FromRoute]Guid id)
        {
            var managerResult = _articlesManager.RemoveArticle(id);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }


        [HttpPost]
        [DisableRequestSizeLimit]
        public IActionResult Edit([FromBody] ArticleDto model)
        {
            var serverPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "articles", "dynamic");
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
            if (!string.IsNullOrEmpty(model.Image)  && model.Image.Contains("base64"))
            {
                var decodeResult = Base64ToImage(model.Image);
                var mainPath = Path.Combine(serverPath, $"{model.Route}.webp");
                var ratioX = (double)800 / decodeResult.Item1.Width;
                var ratioY = (double)400 / decodeResult.Item1.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(decodeResult.Item1.Width * ratio);
                var newHeight = (int)(decodeResult.Item1.Height * ratio);

                decodeResult.Item1.Mutate(x => x.Resize(newWidth, newHeight));
                decodeResult.Item1.SaveAsWebp(mainPath);
                model.Image = Url.Content($"/img/articles/dynamic/{model.Route}.webp");
            }
            var articleModel = _mapper.Map<Article>(model);
            var managerResult = _articlesManager.Edit(articleModel);

            if (managerResult.Success) return Json(managerResult);

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }
        public (Image, IImageFormat) Base64ToImage(string base64String)
        {
            var array = base64String.Split(',', StringSplitOptions.RemoveEmptyEntries);
            // Convert base 64 string to byte[]
            var imageBytes = Convert.FromBase64String(array.LastOrDefault());
            // Convert byte[] to Image
            using var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            var image = Image.Load(ms, out var format);
            return (image, format);
        }
    }
}
