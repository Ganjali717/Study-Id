using System.Net;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.ColorSpaces;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Extentions;
using StudyId.Entities.Tasks;
using StudyId.Models.Dto;
using StudyId.Models.Dto.Admin.Organizations;
using StudyId.Models.Dto.Admin.Tasks;
using StudyId.Models.Dto.Applications;
using Task = StudyId.Entities.Tasks.Task;

namespace StudyId.WebApplication.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class TasksController : Controller
    {
        private readonly ITasksManager _tasksManager;
        private readonly IMapper _mapper;

        public TasksController(ITasksManager tasksManager, IMapper mapper)
        {
            _tasksManager = tasksManager;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Search([FromBody] TaskSearchDto model)
        {
            var managerResult = _tasksManager.GetTasks(model.Q, model.FromValue, model.ToValue, model.OrderBy, model.OrderAsc, model.StatusValue, model.Offset, model.Page, model.Take);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            var dtoResult = _mapper.Map<PagedManagerResult<IList<TaskDto>>>(managerResult);
            return Json(dtoResult);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CopyApp([FromRoute] Guid id)
        {
            var managerResult = _tasksManager.GetTask(id);
            if (!managerResult.Success)
            {
                return RedirectToAction("Create");
            }
            var model = _mapper.Map<TaskDto>(managerResult.Data);
            model.Id = null;
            return View("Edit", model);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Edit");
        }

        [HttpPost]
        public IActionResult Create([FromBody] TaskDto model)
        {
            var application = _mapper.Map<Entities.Tasks.Task>(model);
            var managerResult = _tasksManager.CreateOrUpdate(application);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<TaskDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpGet]
        public IActionResult Edit([FromRoute] Guid id)
        {
            var managerResult = _tasksManager.GetTask(id);
            if (!managerResult.Success)
            {
                return RedirectToAction("Create");
            }

            var model = _mapper.Map<TaskDto>(managerResult.Data);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] TaskDto model)
        {
            var application = _mapper.Map<Entities.Tasks.Task>(model);
            var managerResult = _tasksManager.CreateOrUpdate(application);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<TaskDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }


        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 60)]
        [AllowAnonymous]
        public IActionResult Dictionaries()
        {
            var statuses = Enum.GetValues<Status>().Select(val => new DictionaryDto() { Title = val.GetDisplayName(), Value = val.ToString() }).ToList();
            var applications = _tasksManager.GetApplications().Data;
            var allApps = _mapper.Map<List<ApplicationDto>>(applications);
            var courses = _tasksManager.GetCourses().Data;
            return Json(new { statuses, courses, allApps });
        }

        [HttpPost]
        [Route("/tasks/change-status/{id}")]
        public IActionResult ChangeStatus([FromRoute] Guid id, [FromForm] Status status)
        {
            var managerResult = _tasksManager.ChangeStatus(id, status);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }


        [HttpPost]
        public IActionResult Remove([FromRoute] Guid id)
        {
            var managerResult = _tasksManager.RemoveTask(id);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        
    }
}
