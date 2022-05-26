using StudyId.Entities.Security;

namespace StudyId.Models.Dto.Admin.Accounts
{
    public class AdminAccountInviteDto
    {
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; }
        public Role? RoleValue
        {
            get
            {
                if (string.IsNullOrEmpty(Role)) return null;
                return Enum.Parse<Role>(Role);
            }
        }
    }
}
