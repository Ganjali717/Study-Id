namespace StudyId.Models.Dto.Admin.Articles
{
    public class ArticlesSearchDto:SearchDto
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
        public string? Category { get; set; }
        public Guid? CategoryValue
        {
            get
            {
                if (Guid.TryParse(Category, out var category))
                {
                    return category;
                }
                return null;
            }
        }

    }
}
