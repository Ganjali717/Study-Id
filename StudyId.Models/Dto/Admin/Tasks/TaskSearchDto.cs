using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyId.Entities.Tasks;

namespace StudyId.Models.Dto.Admin.Tasks
{
    public class TaskSearchDto: SearchDto
    {
        public string? From { get; set; }
        public DateTime? FromValue
        {
            get
            {
                if (DateTime.TryParse(From, out var date))
                {
                    return date;
                }
                return null;
            }
        }
        public string? To { get; set; }
        public DateTime? ToValue
        {
            get
            {
                if (DateTime.TryParse(To, out var date))
                {
                    return date;
                }
                return null;
            }
        }
        public string? Status { get; set; }
        public Status? StatusValue
        {
            get
            {
                if (Enum.TryParse<Status>(Status, out var status))
                {
                    return status;
                }
                return null;
            }
        }

        public string? Offset { get; set; }
    }
}
