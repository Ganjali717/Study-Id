using System.ComponentModel.DataAnnotations;

namespace StudyId.Models.Dto.Admin.Articles
{
    public class ArticleCategoryDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
    }
}
