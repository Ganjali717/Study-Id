using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Extentions;
using StudyId.Entities.Organizations;
using StudyId.Models.Dto;
using StudyId.Models.Dto.Admin.Organizations;
using StudyId.Models.Dto.Applications;

namespace StudyId.WebApplication.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class OrganizationsController : Controller
    {
        private readonly IOrganizationsManager _organizationsManager;
        private readonly IMapper _mapper;

        public OrganizationsController(IOrganizationsManager organizationsManager, IMapper mapper)
        {
            _organizationsManager = organizationsManager;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search([FromBody] OrganizationsSearchDto model)
        {   
            var managerResult = _organizationsManager.GetOrganizations(model.Q ,model.FromValue, model.ToValue, model.OrderBy, model.OrderAsc, model.StatusValue, model.Offset, model.Page, model.Take);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            var dtoResult = _mapper.Map<PagedManagerResult<IList<OrganizationDto>>>(managerResult);
            return Json(dtoResult);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View("Edit");
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrganizationDto model)
        {
            var application = _mapper.Map<Organization>(model);
            var managerResult = _organizationsManager.CreateOrUpdate(application);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<OrganizationDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpGet]
        public IActionResult Edit([FromRoute] Guid id)
        {
            var managerResult = _organizationsManager.GetOrganization(id);
            if (!managerResult.Success)
            {
                return RedirectToAction("Create");
            }

            var model = _mapper.Map<OrganizationDto>(managerResult.Data);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] OrganizationDto model)
        {
            var application = _mapper.Map<Organization>(model);
            var managerResult = _organizationsManager.CreateOrUpdate(application);
            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<OrganizationDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 60)]
        [AllowAnonymous]
        public IActionResult Dictionaries()
        {
            var courses = _organizationsManager.GetCourses().Data;
            var statuses = Enum.GetValues<Status>().Select(val => new DictionaryDto() { Title = val.GetDisplayName(), Value = val.ToString() }).ToList();
            return Json(new { statuses, courses });
        }


        [HttpPost("/organizations/upload")]
        public async Task<IActionResult> Upload(IFormCollection formFile)
        {
            const string pathToSave = @"wwwroot\img\organizations\documents";
            long size = formFile.Files.Sum(f => f.Length);
            if (size <= 0) return BadRequest("Empty file");

            foreach (var file in formFile.Files)
            {
                var originalFileName = Path.GetFileName(file.FileName);

                var uniqueFilePath = Path.Combine(Directory.GetCurrentDirectory(), pathToSave, originalFileName);

                using (var stream = System.IO.File.Create(uniqueFilePath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Ok();
        }

        [HttpGet("organizations/download/{downloadFile}")]
        public IActionResult DownloadFile([FromRoute]string downloadFile)
        {
            string link = @"wwwroot\img\organizations\documents\" + downloadFile;
            if (!System.IO.File.Exists(link))
            {
                return BadRequest("File doesn't exist, please try again.");
            }
            var net = new WebClient();
            var data = net.DownloadData(link);
            var content = new System.IO.MemoryStream(data);
            var contentType = "text/plain";
            var fileName = "example.txt";
            return File(content, contentType, fileName);
        }

        [HttpGet("organizations/delete/{deletedFile}")]
        public IActionResult RemoveFile([FromRoute]string deletedFile)
        {
            string link = @"wwwroot\img\organizations\documents\" + deletedFile;
            if (!System.IO.File.Exists(link))
            {
                return BadRequest("File doesn't exist, please try again.");
            }
            System.IO.File.Delete(link);
            return Ok();
        }

        [HttpPost]
        [Route("/organizations/change-status/{id}")]
        public IActionResult ChangeStatus([FromRoute] Guid id, [FromForm] Status status)
        {
            var managerResult = _organizationsManager.ChangeStatus(id, status);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpPost]
        public IActionResult Remove([FromRoute] Guid id)
        {
            var managerResult = _organizationsManager.RemoveOrganization(id);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }
    }
}
