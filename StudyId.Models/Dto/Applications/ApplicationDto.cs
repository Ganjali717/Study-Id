using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.Models.Dto.Applications
{
    public class ApplicationDto
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Guid? Course
        {
            get
            {
                if (Guid.TryParse(CourseId, out var course))
                {
                    return course;
                }
                return null;
            }
        }

        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string? StartDate { get; set; }
        public string? Status { get; set; }
        public long? HubSpotContactId { get; set; }
        public long? HubSpotDealId { get; set; }
        public long? HubSpotCompanyId { get; set; }

        public DateTime? StartDateValue
        {
            get
            {
                if (DateTime.TryParse(StartDate, out var date))
                {
                    return date;
                }
                return null;
            }
        }

        public bool AustralianResident { get; set; }
        public string? Created { get; set; }
        public string? Updated { get; set; }
    }
}
