using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StudyId.Entities.Applications;

namespace StudyId.Entities.Organizations
{
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string TaxNumber { get; set; }
        public string? Phone { get; set; }
        public Status Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public ICollection<OrganizationDocument> Documents { get; set; }
        public ICollection<OrganizationPerson> Persons { get; set; }
        public ICollection<OrganizationCourse> Courses { get; set; }

    }

    
    public enum Status : int
    {
        [Display(Name = "Active")]
        Active = 0,
        [Display(Name = "On Hold")]
        OnHold = 1,
        [Display(Name = "Archived")]
        Archived = 2
    }
    
}
