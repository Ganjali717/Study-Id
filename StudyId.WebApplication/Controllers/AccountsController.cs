using System.Net;
using AutoMapper;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyId.Data.Managers;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Security;
using StudyId.Models.Dto;
using StudyId.Models.Dto.Admin.Accounts;
namespace StudyId.WebApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly IAccountsManager _accountsManager;
        private readonly IMapper _mapper;
        // GET: AccountsController
        public AccountsController(IAccountsManager accountsManager, IMapper mapper)
        {
            _accountsManager = accountsManager;
            _mapper = mapper;
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Search([FromBody] AdminSearchAccountsDto model)
        {
            var managerResult = _accountsManager.GetAccounts(model.Q, model.RoleValue, model.StatusValue, model.OrderBy, model.OrderAsc, model.Page, model.Take);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            var dtoResult = _mapper.Map<PagedManagerResult<IList<AdminAccountDto>>>(managerResult);
            return Json(dtoResult);
        }

        [HttpPost]
        public IActionResult Remove([FromRoute]Guid id)
        {
            var managerResult = _accountsManager.RemoveAccount(id);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        
        [HttpPost]
        public IActionResult Invite([FromBody] AdminAccountInviteDto[] items)
        {
            var accounts = items.Select(x => _mapper.Map<Account>(x)).ToList();
            var users = _accountsManager.GetAllAccounts();
            if (users.Any(x=> accounts.Any(y => y.Email == x.Email)) == false)
            {
                var baseUrlLink = $"{Url.Action("Invite", "auth", null, Request.Scheme, null)}/" + "{0}";
                var managerResult = _accountsManager.Invite(accounts, baseUrlLink);
                if (managerResult.Success) return Json(managerResult);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }

            return BadRequest($"This email {items.FirstOrDefault().Email}, is used by another account");
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 60)]
        public IActionResult Dictionaries()
        {
            var roles = Enum.GetValues<Role>().Select(role => new DictionaryDto() {Title = role.ToString(), Value = role.ToString()}).ToList();
            var statuses = Enum.GetValues<Status>().Select(status => new DictionaryDto() {Title = status.ToString(), Value = status.ToString()}).ToList();
            return Json(new { roles, statuses });
        }


        [HttpPost]
        [Route("/accounts/change-status/{id}")]
        public IActionResult ChangeStatus([FromRoute] Guid id, [FromForm] Status status)
        {
            var managerResult = _accountsManager.ChangeStatus(id, status);
            if (managerResult.Success) return Json(managerResult);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpPost]
        [Route("/accounts/exist")]
        public IActionResult Exist([FromBody]AccountExistDto existModel)
        {
            var managerResult = _accountsManager.IsAccountExist(existModel.Email);
            if (managerResult.Success) return Json(managerResult.Data);
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }
    }
}
