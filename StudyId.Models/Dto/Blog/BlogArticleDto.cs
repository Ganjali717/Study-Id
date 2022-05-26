using StudyId.Entities.Articles;
using StudyId.Models.Dto.Admin.Articles;

namespace StudyId.Models.Dto.Blog
{
    public class BlogArticleDto
    {
        public ArticleDto Article { get; set; }
        public List<ArticleDto> RelatedArticles { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<string> Tags { get; set; }
    }
}
