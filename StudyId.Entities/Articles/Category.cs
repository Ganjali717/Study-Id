using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudyId.Entities.Articles
{
    [Index("RouteKey", IsUnique = true, Name="IX_RouteKey")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Route { get; set; }
        public long RouteKey { get; set; }
    }
}
