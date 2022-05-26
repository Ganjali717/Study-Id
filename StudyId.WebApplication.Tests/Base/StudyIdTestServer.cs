using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyId.Data.DatabaseContext;
using StudyId.SmtpManager;

namespace StudyId.WebApplication.Tests.Base
{
    public class StudyIdTestServer<TStartup> : WebApplicationFactory<TStartup>  where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var path = Assembly.GetAssembly(typeof(Program)).Location;
            var solutionPath = Directory.GetParent(path).Parent.Parent.Parent.Parent.ToString();
            var configFolder = Path.Combine(solutionPath, "StudyId.WebApplication");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<StudyIdDbContext>));

                services.Remove(descriptor);

                services.AddDbContext<StudyIdDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<StudyIdDbContext>();
                    db.Database.EnsureCreated();

                    try
                    {
                        Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
        }
    }
}
    



