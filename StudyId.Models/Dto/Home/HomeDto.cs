using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyId.Models.Dto.Admin.Articles;

namespace StudyId.Models.Dto.Home
{
    public class HomeDto
    {
        public List<ArticleDto> TopArticles { get; set; }
    }
}
