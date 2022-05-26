namespace StudyId.Models.Dto.Categories
{
    public class CategoryDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public bool? Edit { get; set; }
        public bool? DelPopup { get; set; }
        public string Route { get; set; }
    }
}
