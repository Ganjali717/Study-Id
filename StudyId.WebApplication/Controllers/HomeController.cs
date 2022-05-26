using System.Collections;
using Microsoft.AspNetCore.Mvc;
using StudyId.WebApplication.Models;
using System.Diagnostics;
using System.Net;
using AutoMapper;
using NuGet.Protocol;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities;
using StudyId.Entities.Applications;
using StudyId.Entities.Security;
using StudyId.Models.Dto.Admin.Accounts;
using StudyId.Models.Dto.Admin.Articles;
using StudyId.Models.Dto.Applications;
using StudyId.Models.Dto.Home;
using StudyId.SmtpManager;
using Status = StudyId.Entities.Security.Status;

namespace StudyId.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApplicationsManager _applicationsManager;
        private readonly IArticlesManager _articlesManager;
        private readonly IAccountsManager _accountsManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly ISmtpManager _smtpManager;
        private readonly IServiceProvider _services;

        public HomeController(ILogger<HomeController> logger, IApplicationsManager applicationsManager, IMapper mapper, IArticlesManager articlesManager, IWebHostEnvironment environment, ISmtpManager smtpManager, IAccountsManager accountsManager, IServiceProvider services)
        {
            _logger = logger;
            _applicationsManager = applicationsManager;
            _mapper = mapper;
            _articlesManager = articlesManager;
            _environment = environment;
            _smtpManager = smtpManager;
            _accountsManager = accountsManager;
            _services = services;
        }
        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        public IActionResult Index()
        {
            var model = new HomeDto();
            model.TopArticles = _mapper.Map<List<ArticleDto>>(_articlesManager.GetLastPosts().Data);
            return View(model);
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications")]
        public IActionResult Qualifications()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("about-us")]
        public IActionResult AboutUs()
        {
            return View();
        }


        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("contact-us")]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [Route("enroll")]
        public IActionResult Enroll([FromBody] ApplicationDto model)
        {
            var application = _mapper.Map<Application>(model);
            var managerResult = _applicationsManager.CreateOrUpdate(application);
            var users = _accountsManager.GetAccounts(null, Role.Manager, null, null, true, 1);
            var mappedResult = _mapper.Map<PagedManagerResult<IList<AdminAccountDto>>>(users);
            Parallel.ForEach(mappedResult.Data,
                user =>
                {
                    var smtpManager = _services.GetRequiredService<ISmtpManager>();
                    var link = user.FirstName + " " + user.LastName;
                    var keys = new Hashtable { { "UserName", link } };
                    var mailTemplate = smtpManager.GenerateHtmlBody("Applications.ApplicationInvite.html", keys);
                    smtpManager.Send(user.Email, "StudyID: a New Application is Submitted", mailTemplate.Data);
                });

            if (managerResult.Success) return Json(_mapper.Map<ManagerResult<ApplicationDto>>(managerResult));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(managerResult.Message);
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/business")]
        public IActionResult Business()
        {
            return View();
        }
        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/digital-Media-and-information-technology")]
        public IActionResult DigitalMediaAndInformationTechnology()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/early-childhood-education-and-care")]
        public IActionResult EarlyChildhoodEducationAndCare()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/english")]
        public IActionResult English()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/environment-management-and-sustainability")]
        public IActionResult EnvironmentManagementSustainability()
        {
            return View();
        }


        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/hospitality-and-culinary")]
        public IActionResult HospitalityCulinary()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/leadership-and-management")]
        public IActionResult LeadershipManagement()
        {
            return View();
        }


        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/marine-habitat-conservation-and-restoration")]
        public IActionResult MarineHabitatConservationRestoration()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("qualifications/marketing-and-communications")]
        public IActionResult MarketingCommunications()
        {
            return View();
        }

        [HttpGet]
        [Route("faqs")]
        public IActionResult Faqs()
        {
            return RedirectToAction("Faq");
        }


        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
        [Route("faq")]
        public IActionResult Faq()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 600)]
        [Route("/sitemap.xml")]
        public SiteMapResult SiteMap()
        {
            var articles = _articlesManager.GetArticles(null, null, null, null, null, null, 1, int.MaxValue, null);
            var siteMap = new SiteMapResult(_environment, articles.Data.ToList());
            return siteMap;
        }
    }
}