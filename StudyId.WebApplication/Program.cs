using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Managers;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities.Extentions;
using System.Configuration;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using Microsoft.AspNetCore.Localization;
using StudyId.HubSpotManager;
using StudyId.Models.Automapper;
using StudyId.SmtpManager;
using StudyId.WebApplication.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});
/*var test = "very-very-long-string-for-route-for-understanding-if-this-will-work-or-not".GetIntHash();*/
CultureInfo[] supportedCultures = new[]
{
    new CultureInfo("en-AU")
};
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-AU");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider()
    };
});
builder.Services.AddDbContext<StudyIdDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudyIdDbContext")));
//Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => //CookieAuthenticationOptions
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/auth/login");
    });
builder.Services.AddAutoMapper(c => c.AddProfile<BaseAutomapperProfile>(), typeof(Program));
// Add services to the container.
builder.Logging.AddLog4Net("log4net.config");
builder.Services.AddResponseCaching();
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes =
        ResponseCompressionDefaults.MimeTypes.Concat(
            new[]
            {
                "font/woff2",
                "application/font-woff2",
                "application/font-woff",
                "application/font",
                "application/javascript",
                "application/x-javascript",
                "image/jpg",
                "image/jpeg",
                "image/png",
                "image/svg+xml",
                "video/mp4",
                "image/x-icon",
                "image/webp",
                "application/javascript; charset=utf-8"
            });
});
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<StudyIdDbContext>();
builder.Services.AddScoped<IAccountsManager, AccountsManager>();
builder.Services.AddScoped<ISmtpManager, SendPulseManager>();
builder.Services.AddScoped<IArticlesManager, ArticlesManager>();
builder.Services.AddScoped<IApplicationsManager, ApplicationsManager>();
builder.Services.AddScoped<ICategoriesManager, CategoriesManager>();
builder.Services.AddScoped<IOrganizationsManager, OrganizationsManager>();
builder.Services.AddScoped<ITasksManager, TasksManager>();
builder.Services.AddScoped<ICacheManager, CacheManager>();
builder.Services.AddScoped<IHubSpotManager, HubSpotManager>();

var app = builder.Build();
app.UseRequestLocalization();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseResponseCompression();

app.UseResponseCaching();
app.UseAuthentication();    
app.UseAuthorization(); 
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        switch (Path.GetExtension(ctx.File.Name))
        {
            case ".svg":  case ".webp": case ".jpg":case ".png":case ".jpeg":case ".tiff": case ".woff": case ".woff2":
                // Cache static files for 30 days
                ctx.Context.Response.Headers.Append("Cache-Control", "max-age=31536000");
                ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddYears(1).ToString("R", CultureInfo.InvariantCulture));
                break;
            case ".js": case ".css":
                // Cache static files for 30 days
                ctx.Context.Response.Headers.Append("Cache-Control", "max-age=31536000");
                ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddYears(1).ToString("R", CultureInfo.InvariantCulture));
                break;
            default:
                break;
        }

    }
});
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.EnsureMigrationOfContext<StudyIdDbContext>();
app.Run();

public partial class Program
{
    
}