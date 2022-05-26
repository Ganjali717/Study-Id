using System.Collections;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Managers;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Security;
using StudyId.Models.Dto;
using StudyId.Models.Dto.Auth;
using StudyId.SmtpManager;
using StudyId.WebApplication.Models;

namespace StudyId.WebApplication.Controllers
{
    public class AuthController : Controller
    {
        private const string schema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";
        private readonly IAccountsManager _accountsManager;
        private readonly ISmtpManager _smtpManager;
        private readonly IMapper _mapper;

        public AuthController(IAccountsManager accountsManager, ISmtpManager smtpManager, IMapper mapper)
        {
            _accountsManager = accountsManager;
            _smtpManager = smtpManager;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                return RedirectToAction("Index", "Accounts");
            }

            return View("Login", returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto data)
        {
            var managerResult = _accountsManager.Login(data.Username, data.Password);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            await Authenticate(managerResult.Data);
            managerResult.Message = GetRedirectUrl(data.ReturnUrl, managerResult.Data.Role);
            return Json(managerResult);


        }
        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Forgot([FromBody] ForgotPasswordDto model)
        {
            var link = Url.Action("reset","auth", null, Request.Scheme)+"/{0}";
            var test = Url.RouteUrl("reset");
            var managerResult = _accountsManager.ResetPasswordRequest(model.Email, link);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            return Json(managerResult);
        }
        [HttpGet]
        [Route("auth/invite/{token}")]
        public IActionResult Invite([FromRoute] string token)
        {
            var result = _accountsManager.GetAccountBySecurityToken(token);

            if (result.Data.SecurityTokenExpired <= DateTimeOffset.UtcNow )
            {
                return RedirectToAction("ErrorExpired", "Auth");
            }

            if (result.Data == null)
            {
                return RedirectToAction("ErrorDeleted", "Auth");
            }

            if (result.Data.Status == Status.Active)
            {
                return RedirectToAction("ErrorRegister", "Auth");
            }

            if (result.Success)
            {
                var model = _mapper.Map<InviteRegisterDto>(result.Data);
                model.Password = token;
                return View("Invite", model);
            }
            return RedirectToAction("Error", "Home", new { code = result.Message });
        }
        [HttpPost]
        [Route("auth/invite/{token}")]
        public IActionResult Invite([FromBody] InviteRegisterDto model, [FromRoute] string token)
        {
            var managerResult = _accountsManager.FinishRegistration(token, model.FirstName, model.LastName, model.Password);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            Authenticate(managerResult.Data).Wait();
            var result = new ManagerResult() { Success = true, Message = GetRedirectUrl(null, managerResult.Data.Role) };
            return Json(result);
        }

        [HttpGet]
        [Route("auth/reset/{token}")]
        public IActionResult Reset([FromRoute] string token)
        {
            var result = _accountsManager.GetAccountBySecurityToken(token);
            if (result.Success)
            {
                return View("ResetPassword", token);
            }

            return RedirectToAction("Error", "Home", new { code = result.Message });
        }


        [HttpPost]
        [Route("auth/reset/{token}")]
        public IActionResult Reset([FromBody] ResetPassordDto data, [FromRoute]string token)
        {
            var managerResult = _accountsManager.ResetPassword(token, data.NewPassword);
            if (!managerResult.Success)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(managerResult.Message);
            }
            return Json(managerResult.Success);
        }
        private async Task Authenticate(Account account)
        {

            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, account.Email ?? string.Empty),
                new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new(ClaimTypes.Email, account.Email ?? string.Empty),
                new(ClaimTypes.GivenName, account.FirstName ?? string.Empty),
                new(ClaimTypes.Surname, account.LastName ?? string.Empty),
                new(schema+"fullname", $"{account.FirstName} {account.LastName}"),
                new(ClaimTypes.Role, account.Role.ToString())
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            HttpContext.SignOutAsync().Wait();
            return RedirectToAction("login", "auth");
        }


        public IActionResult ErrorRegister()
        {
            return View();
        }

        public IActionResult ErrorDeleted()
        {
            return View();
        }

        public IActionResult ErrorExpired()
        {
            return View();
        }
        private string GetRedirectUrl(string? returnUrl, Role role)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return returnUrl;
            }
            else
            {
                switch (role)
                {
                    case Role.Admin:
                        return Url.Action("index", "accounts", null, Request.Scheme, null) ?? string.Empty;
                        break;
                    case Role.Manager:
                        return Url.Action("index", "applications", null, Request.Scheme, null) ?? string.Empty;
                        break;
                    default:
                        return Url.Action("index", "accounts", null, Request.Scheme, null) ?? string.Empty;
                        break;
                }

            }
        }
    }
}
