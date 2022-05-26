using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudyId.Entities.Courses;

namespace StudyId.Entities.Applications
{
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public Status Status { get; set; }
        public DateTime? StartDate { get; set; }
        [NotMapped]
        public string StartDateString => StartDate.HasValue ? StartDate.Value.ToString("dd/MM/yyyy") : string.Empty;
        public bool AustralianResident { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public long? HubSpotContactId { get; set; }
        public long? HubSpotDealId { get; set; }
        public long? HubSpotCompanyId { get; set; }
        [ForeignKey("Course")]
        public Guid? CourseId { get; set; }
        public virtual Course? Course { get; set; }
    }

    public enum Status:int
    {
        [Display(Name = "New")]
        New = 0,
        [Display(Name = "Processed")]
        Processed = 1
    }
}
