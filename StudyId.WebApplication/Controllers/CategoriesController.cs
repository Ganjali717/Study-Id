using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Articles;
using StudyId.Models.Dto.Applications;
using StudyId.Models.Dto.Categories;

namespace StudyId.WebApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoriesManager _categoriesManager;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoriesManager categoriesManager, IMapper mapper)
        {
            _categoriesManager = categoriesManager;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Search([FromBody] CategoriesSearchDto model)
        {
            var managerResult = _categoriesManager.GetCategories(model.Q,  model.OrderBy, model.OrderAsc, model.Page, model.Take);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            var dtoResult = _mapper.Map<PagedManagerResult<IList<CategoryDto>>>(managerResult);
            return Json(dtoResult);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CategoryDto model)
        {
            var category = _mapper.Map<Category>(model);
            var managerResult = _categoriesManager.Create(category);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<CategoryDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] CategoryDto model)
        {
            var category = _mapper.Map<Category>(model);
            var managerResult = _categoriesManager.Update(category);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<CategoryDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpPost]
        public IActionResult Delete([FromRoute]Guid id)
        {
            var managerResult = _categoriesManager.Delete(id);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }
    }
}
