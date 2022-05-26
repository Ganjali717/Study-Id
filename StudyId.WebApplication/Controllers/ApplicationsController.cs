using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Applications;
using StudyId.Entities.Extentions;
using StudyId.Entities.Security;
using StudyId.Models.Dto;
using StudyId.Models.Dto.Applications;
using Status = StudyId.Entities.Applications.Status;

namespace StudyId.WebApplication.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationsManager _applicationsManager;
        private readonly IMapper _mapper;

        public ApplicationsController(IApplicationsManager applicationsManager, IMapper mapper)
        {
            _applicationsManager = applicationsManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search([FromBody] ApplicationsSearchDto model)
        {
            var managerResult = _applicationsManager.GetApplications(model.Q, model.IsAustralian, model.Course, model.FromValue, model.ToValue, model.OrderBy, model.OrderAsc, model.StatusValue, model.Page, model.Take);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            var dtoResult = _mapper.Map<PagedManagerResult<IList<ApplicationDto>>>(managerResult);
            return Json(dtoResult);
        }

        [HttpGet]
        public IActionResult Detail([FromRoute] Guid id)
        {
            var managerResult = _applicationsManager.GetApplication(id);
            if (!managerResult.Success)
            {
                return RedirectToAction("Create");
            }
            var model = _mapper.Map<ApplicationDto>(managerResult.Data);
            return View(model);
        }


        [HttpGet]
        public IActionResult CopyApp([FromRoute] Guid id)
        {
            var managerResult = _applicationsManager.GetApplication(id);
            if (!managerResult.Success)
            { 
                return RedirectToAction("Create");
            }
            var model = _mapper.Map<ApplicationDto>(managerResult.Data);
            model.Id = null;
            return View("Edit", model);
            
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Edit");
        }

        [HttpPost]
        public IActionResult Create([FromBody] ApplicationDto model)
        {
            var application = _mapper.Map<Application>(model);
            var managerResult = _applicationsManager.CreateOrUpdate(application);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<ApplicationDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return  Json(managerResult.Message);
            
        }

        [HttpGet]
        public IActionResult Edit([FromRoute] Guid id)
        {
            var managerResult = _applicationsManager.GetApplication(id);
            if (!managerResult.Success)
            {
                return RedirectToAction("Create");
            }

            var model = _mapper.Map<ApplicationDto>(managerResult.Data);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] ApplicationDto model)
        {
            var application = _mapper.Map<Application>(model);
            var managerResult = _applicationsManager.CreateOrUpdate(application);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<ApplicationDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpPost]
        [Route("/applications/change-status/{id}")]
        public IActionResult ChangeStatus([FromRoute]Guid id,[FromForm] Status status)
        {
            var managerResult = _applicationsManager.ChangeStatus(id,status);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpPost]
        public IActionResult Remove([FromRoute] Guid id)
        {
            var managerResult = _applicationsManager.RemoveApplication(id);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int) HttpStatusCode.BadRequest; 
            return Json(managerResult.Message);
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 60)]
        [AllowAnonymous]
        public IActionResult Dictionaries()
        {
            var courses = _applicationsManager.GetCourses().Data;
            var tasks = _applicationsManager.GetTasks().Data;
            var statuses = Enum.GetValues<Status>().Select(val => new DictionaryDto() {Title = val.GetDisplayName(), Value = val.ToString()}).ToList();
            return Json(new { courses, statuses, tasks });
        }


      
    }
}
