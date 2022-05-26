using Microsoft.EntityFrameworkCore;
using StudyId.Data.DatabaseContext;

namespace StudyId.WebApplication.Extensions
{
    public static class EnsureMigrationExtention
    {
        public static void EnsureMigrationOfContext<T>(this IApplicationBuilder app) where T:DbContext
        {
            var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<T>();
            context.Database.Migrate();
        }
    }
}
