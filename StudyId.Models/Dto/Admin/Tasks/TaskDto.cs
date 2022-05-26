using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.Models.Dto.Admin.Tasks
{
    public class TaskDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string? DueDate { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CourseName { get; set; }
        public DateTime? DueDateValue
        {
            get
            {
                if (DateTime.TryParse(DueDate, out var date))
                {
                    return date;
                }
                return null;
            }
        }
        public string? Created { get; set; }
        public string? Updated { get; set; }
        public string Description { get; set; }
        public string TaskEmail { get; set; }
        public string TaskPhone { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
