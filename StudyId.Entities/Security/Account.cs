using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyId.Entities.Security
{
    public class Account
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Role Role { get; set; }
        public Status Status { get; set; }
        public string? Password { get; set; }
        public string? SecurityToken { get; set; }
        public DateTimeOffset? SecurityTokenExpired { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        

    }

    public enum Role
    {
        Admin = 1,
        Manager = 2,
    }

    public enum Status
    {
        Active = 0,
        Inactive = 1,
        Invited = 2
    }
}
