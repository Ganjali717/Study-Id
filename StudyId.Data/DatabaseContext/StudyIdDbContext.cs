using Microsoft.EntityFrameworkCore;
using StudyId.Entities.Applications;
using StudyId.Entities.Articles;
using StudyId.Entities.Courses;
using StudyId.Entities.Organizations;
using StudyId.Entities.Security;
using StudyId.Entities.Tasks;

namespace StudyId.Data.DatabaseContext
{
    public class StudyIdDbContext : DbContext
    {
#pragma warning disable CS8618
        public StudyIdDbContext(DbContextOptions<StudyIdDbContext> options)
#pragma warning restore CS8618
            : base(options)
        { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationCourse> OrganizationsCourses { get; set; }
        public DbSet<OrganizationPerson> OrganizationPersons { get; set; }
        public DbSet<OrganizationDocument> OrganizationDocuments { get; set; }
        public DbSet<StudyId.Entities.Tasks.Task> Tasks { get; set; }
        


        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Article>()
            .Property(b => b.Created)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");
        }
    }
}
