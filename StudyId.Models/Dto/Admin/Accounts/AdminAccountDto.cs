namespace StudyId.Models.Dto.Admin.Accounts
{
    public class AdminAccountDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Created  { get; set; }
    }
}
