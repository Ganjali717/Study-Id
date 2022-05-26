using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyId.Entities;
using StudyId.Entities.Articles;

namespace StudyId.Models.Dto.Blog
{
    public class BlogSearchDto:SearchDto
    {
        public Guid? CategoryId { get; set; }
        public string? Tag  { get; set; }

        public List<long> Pages
        {
            get
            {
                var pages = new List<long>();
                var pagesCount = (int)Math.Ceiling((double)Items.Total / Items.Take);
                for (var i = 0; i < pagesCount; i++)
                {
                    pages.Add(i + 1);
                }
                return pages;
            }
        }

        public IList<Category> Categories { get; set; }
        public IList<string> Tags { get; set; }
        public PagedManagerResult<IList<StudyId.Models.Dto.Admin.Articles.ArticleDto>> Items { get; set; }
        public bool NoIndex { get; set; }
    }
}
