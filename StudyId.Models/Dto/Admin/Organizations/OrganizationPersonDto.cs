using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.Models.Dto.Admin.Organizations
{
    public class OrganizationPersonDto
    {
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
