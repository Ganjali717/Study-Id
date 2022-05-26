using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.Models.Dto.Admin.Organizations
{
    public class OrganizationCourseDto
    {
        public Guid? CourseId { get; set; }
        public string? LocalName { get; set; }
        public int? Duration { get; set; }
        public decimal? Price { get; set; }
    }
}
