using StudyId.Entities.Applications;
namespace StudyId.Models.Dto.Applications
{
    public class ApplicationsSearchDto : SearchDto
    {
        public string? From { get; set; }
        public bool? IsAustralian { get; set; }
        public Guid?[] Course { get; set; }
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
    }
}
