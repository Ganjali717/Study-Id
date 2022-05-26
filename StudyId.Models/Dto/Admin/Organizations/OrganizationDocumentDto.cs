using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.Models.Dto.Admin.Organizations
{
    public class OrganizationDocumentDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
