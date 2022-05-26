using AutoMapper;
using StudyId.Entities;
using StudyId.Entities.Applications;
using StudyId.Entities.Articles;
using StudyId.Entities.Extentions;
using StudyId.Entities.Organizations;
using StudyId.Entities.Security;
using StudyId.Entities.Tasks;
using StudyId.HubSpotManager.Models.Contacts.StudentContacts;
using StudyId.HubSpotManager.Models.Deals;
using StudyId.Models.Dto.Admin.Accounts;
using StudyId.Models.Dto.Admin.Articles;
using StudyId.Models.Dto.Admin.Organizations;
using StudyId.Models.Dto.Admin.Tasks;
using StudyId.Models.Dto.Applications;
using StudyId.Models.Dto.Auth;
using StudyId.Models.Dto.Categories;

namespace StudyId.Models.Automapper
{
    public class BaseAutomapperProfile:Profile
    {
        public BaseAutomapperProfile()
        {
            CreateMap<TaskDto, Entities.Tasks.Task>()
                .ForMember(x => x.DueDate, s => s.MapFrom(x => x.DueDateValue))
                .ForMember(x => x.ApplicationId, s => s.MapFrom(x => x.ApplicationId));
            CreateMap<Entities.Tasks.Task, TaskDto>()
                .ForMember(x => x.Created, s => s.MapFrom(x => x.Created.ToString("dd/MM/yyyy HH:mm")))
                .ForMember(x => x.Updated, s => s.MapFrom(x => x.Updated.ToString("dd/MM/yyyy HH:mm")))
                .ForMember(x => x.FirstName, s=> s.MapFrom(x => x.Applications.FirstName))
                .ForMember(x=> x.LastName, s=> s.MapFrom(x=> x.Applications.LastName))
                .ForMember(x => x.ApplicationId, s => s.MapFrom(x => x.ApplicationId));
            CreateMap<OrganizationCourseDto, OrganizationCourse>();
            CreateMap<OrganizationCourse, OrganizationCourseDto>();
            CreateMap<OrganizationPersonDto, OrganizationPerson>();
            CreateMap<OrganizationPerson, OrganizationPersonDto>();
            CreateMap<OrganizationDocumentDto, OrganizationDocument>();
            CreateMap<OrganizationDocument, OrganizationDocumentDto>();
            CreateMap<OrganizationDto, Organization>()
                .ForMember(x => x.Updated, s => s.MapFrom(x => x.UpdateValue))
                .ForMember(x => x.StartDate, s => s.MapFrom(x => x.StartDateValue));
            CreateMap<Organization, OrganizationDto>()
                //.ForMember(x => x.Persons, s => s.MapFrom(x => x.Persons == null ? Array.Empty<Guid>() : x.Persons.Select(x => x.Id).ToArray()))
                //.ForMember(x => x.Courses, s => s.MapFrom(x => x.Courses == null ? Array.Empty<Guid>() : x.Courses.Select(x => x.CourseId).ToArray()))
                .ForMember(x => x.StartDate, s => s.MapFrom(x => x.StartDate.HasValue ? x.StartDate.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(x => x.Updated, s => s.MapFrom(x => x.Updated.ToString("dd/MM/yyyy HH:mm")))
                .ForMember(x => x.Created, s => s.MapFrom(x => x.Created.ToString("dd/MM/yyyy HH:mm")));
            CreateMap<ArticleDto, Article>()
                .ForMember(x => x.PublishOn, s => s.MapFrom(x => x.PublishOnValue))
                .ForMember(x => x.RouteKey, s => s.MapFrom(x => x.Route.GetIntHash()))
                .ForMember(x => x.CategoryId, s => s.MapFrom(x => x.CategoryId));
            CreateMap<Article, ArticleDto>()
                .ForMember(x => x.CategoryId, s => s.MapFrom(x => x.CategoryId))
                .ForMember(x => x.PublishOn, s => s.MapFrom(x => x.PublishOn.HasValue ? x.PublishOn.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(x => x.Created, s => s.MapFrom(x =>  x.Created.ToString("dd/MM/yyyy")))
                .ForMember(x => x.Category, s => s.MapFrom(x =>x.Category!=null ? x.Category.Title : string.Empty))
                .ForMember(x => x.CategoryRoute, s => s.MapFrom(x =>x.Category!=null ? x.Category.Route : string.Empty))
                .ForMember(x => x.IsPermanent, s => s.MapFrom(x =>x.IsPermanent));
            CreateMap<CategoryDto, Category>()
                .ForMember(x => x.RouteKey, s => s.MapFrom(x => x.Route.GetIntHash()));
            CreateMap<Category, CategoryDto>();
            CreateMap<ApplicationDto, Application>()
                .ForMember(x => x.StartDate, s => s.MapFrom(x => x.StartDateValue))
                .ForMember(x => x.CourseId, s => s.MapFrom(x => x.Course))
                .ForMember(x => x.Course, s => s.Ignore());
            CreateMap<Application, ApplicationDto>()
                .ForMember(x=>x.StartDate, s=>s.MapFrom(x=>x.StartDate.HasValue ? x.StartDate.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(x=>x.Created, s=>s.MapFrom(x=>x.Created.ToString("dd/MM/yyyy")))
                .ForMember(x => x.Updated, s => s.MapFrom(x => x.Created.ToString("dd/MM/yyyy")))
                .ForMember(x => x.CourseId, s => s.MapFrom(x => x.CourseId))
                .ForMember(x => x.CourseName, s => s.MapFrom(x => x.Course==null ? String.Empty : x.Course.Title));
            CreateMap<Account, AdminAccountDto>()
                .ForMember(x=>x.Created, s=>s.MapFrom(x=>x.Created.ToString("dd/MM/yyyy  HH:mm")));
            CreateMap<Account, InviteRegisterDto>();
            CreateMap<AdminAccountInviteDto, Account>()
                .ForMember(x=>x.Role, s=>s.MapFrom(x=>x.RoleValue))
                .ForMember(x=>x.FirstName, s=>s.MapFrom(x=>x.FirstName ?? string.Empty))
                .ForMember(x=>x.LastName, s=>s.MapFrom(x=>x.LastName ?? string.Empty))
                .ForMember(x=>x.Password, s=>s.MapFrom(x=>string.Empty));
            CreateMap(typeof(ManagerResult<>), typeof(ManagerResult<>));
            CreateMap(typeof(PagedManagerResult<>), typeof(PagedManagerResult<>));

            //Hubspot region
            CreateMap<Application, DealRequestModel>()
                .ForMember(x => x.Id, s => s.Ignore())
                .ForMember(x => x.Title,
                    s => s.MapFrom(x => $"{x.FirstName} {x.LastName} - {x.Email} - {x.StartDateString}"))
                .ForMember(x => x.PrefferedQualification,
                    s => s.MapFrom(x => x.Course != null ? x.Course.Title : "Not selected"))
                .ForMember(x => x.PrefferedStartDate, s => s.MapFrom(x => x.StartDate))
                .ForMember(x => x.AustraliaResident, s => s.MapFrom(x => x.AustralianResident));
            CreateMap<Application, StudentContactRequestModel>()
                .ForMember(x => x.Id, s => s.Ignore())
                .ForMember(x => x.FirstName, s => s.MapFrom(x => x.FirstName))
                .ForMember(x => x.LastName, s => s.MapFrom(x => x.LastName))
                .ForMember(x => x.Mobile, s => s.MapFrom(x => x.Phone))
                .ForMember(x => x.Email, s => s.MapFrom(x => x.Email));
        }
    }
}
