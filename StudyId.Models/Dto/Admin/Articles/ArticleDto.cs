
namespace StudyId.Models.Dto.Admin.Articles
{
    public class ArticleDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Route { get; set; }
        public string ShortDescription { get; set; }
        public string Body { get; set; }
        public string Image { get; set; } //1000x500
        public string Tags { get; set; }
        public string ImageTags { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Category { get; set; }
        public string? CategoryRoute { get; set; }
        public  string? PublishOn { get; set; }
        public bool? IsPermanent { get; set; }
        public DateTime? PublishOnValue
        {
            get
            {
                if (DateTime.TryParse(PublishOn, out var date))
                {
                    return date;
                }
                return null;
            }
        }
        public string? Status { get; set; }
        public string? Created { get; set; }

    }
}
