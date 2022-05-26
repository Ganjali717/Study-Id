using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using StudyId.Entities.Organizations;

namespace StudyId.Models.Dto.Admin.Organizations
{
    public class OrganizationDto
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public string? StartDate { get; set; }
        public string? Created { get; set; }
        public string? Updated { get; set; }

        public string? PersonName { get; set; }
        public DateTime? UpdateValue
        {
            get
            {
                if (DateTime.TryParse(Updated, out var date))
                {
                    return date;
                }
                return null;
            }
        }
        
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

        public OrganizationPersonDto[]? Persons { get; set; }
        public OrganizationCourseDto[]? Courses { get; set; }
        public OrganizationDocumentDto[]? Documents { get; set; }
    }
}
