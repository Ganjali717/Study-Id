using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyId.Entities.Courses;

namespace StudyId.Entities.Organizations
{
    public class OrganizationCourse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual Organization Organizations { get; set; }
        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }
        public virtual Course Course { get; set; }
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        public string LocalName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }      
    }
}
