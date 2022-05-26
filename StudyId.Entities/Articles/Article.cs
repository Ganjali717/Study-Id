using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudyId.Entities.Articles
{
    [Index("RouteKey", IsUnique = true, Name="IX_RouteKey")]
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public long RouteKey { get; set; }
        public string Route { get; set; }
        public string ShortDescription { get; set; }
        public string Body { get; set; }
        public string Image { get; set; } //1000x500
        public Category? Category { get; set; }
        [ForeignKey("Category")]
        public Guid? CategoryId { get; set; }
        public string Tags { get; set; }
        public string ImageTags { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? PublishOn { get; set; }
        public Status  Status { get; set; }
        public bool IsPermanent { get; set; }
        //================Seo=======================

    }

    public enum Status : int
    {
        Published=0,
        Unpublished=1,
        Archived=2
    }
}
