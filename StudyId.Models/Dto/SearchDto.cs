namespace StudyId.Models.Dto
{
    public class SearchDto
    {
        public string? Q { get; set; }
        public string? OrderBy { get; set; }
        public bool? OrderAsc { get; set; }
        public int Page { get; set; }
        public int Take { get; set; }
    }
}
