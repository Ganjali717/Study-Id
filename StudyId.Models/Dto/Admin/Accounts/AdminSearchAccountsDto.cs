using StudyId.Entities.Security;

namespace StudyId.Models.Dto.Admin.Accounts
{
    public class AdminSearchAccountsDto:SearchDto
    {
        public string? Role { get; set; }

        public Role? RoleValue
        {
            get
            {
                if (string.IsNullOrEmpty(Role)) return null;
                return Enum.Parse<Role>(Role);
            }
        }

        public string? Status { get; set; }

        public Status? StatusValue
        {
            get
            {
                if (string.IsNullOrEmpty(Status)) return null;
                return Enum.Parse<Status>(Status);
            }
        }
    }
}
