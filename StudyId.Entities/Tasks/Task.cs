using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyId.Entities.Applications;
using StudyId.Entities.Courses;

namespace StudyId.Entities.Tasks
{
    public class Task
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public Status Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public string? Description { get; set; }
        public string? TaskEmail { get; set; }
        public string? TaskPhone { get; set; }
        
        public Guid ApplicationId { get; set; }
        public virtual Application Applications { get; set; }
    }

    public enum Status : int
    {

        [Display(Name = "New")]
        New = 0,
        [Display(Name = "In Progress")]
        InProgress = 1,
        [Display(Name = "On Hold")]
        OnHold = 2,
        [Display(Name = "Archived")]
        Archived = 3,
        [Display(Name = "Closed Success")]
        ClosedSuccess = 4
    }
}
