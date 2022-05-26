using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudyId.Entities.Applications;

namespace StudyId.Entities.Courses
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string TgaCode { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}
