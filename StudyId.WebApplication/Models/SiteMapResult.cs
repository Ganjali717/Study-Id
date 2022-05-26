using System.Reflection;
using System.Security.Policy;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using StudyId.Entities.Articles;

namespace StudyId.WebApplication.Models
{
    public class SiteMapResult:ActionResult
    {
        private readonly IWebHostEnvironment _environment;
        private readonly DateTime _lastModifiedDate;
        private readonly List<Article> _articles;
        public SiteMapResult(IWebHostEnvironment environment, List<Article> articles)
        {
            _environment = environment;
            _articles = articles;
            var assembly = Assembly.GetExecutingAssembly();
            FileInfo info = new FileInfo(assembly.Location);
            _lastModifiedDate = info.LastWriteTime;
        }
        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";
            context.HttpContext.Response.Headers.Add("content-disposition", @"attachment;filename=""sitemap.xml""");
            var baseDomain = context.HttpContext.Request.GetDisplayUrl().Replace(context.HttpContext.Request.Path,string.Empty);
            var xws = new XmlWriterSettings { OmitXmlDeclaration = false, Indent = true };
            using (var writer = XmlWriter.Create(context.HttpContext.Response.Body, xws))
            {
                XNamespace blank = XNamespace.Get(@"http://www.sitemaps.org/schemas/sitemap/0.9");
                XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
                var document = new XDocument { Declaration = new XDeclaration("1.0", "utf-8", "yes") };

                var sitemapHead = new XElement(blank + "urlset");
                sitemapHead.Add(new XAttribute("xmlns", blank.NamespaceName));
                sitemapHead.Add(new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
                sitemapHead.Add(new XAttribute(xsi + "schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"));
                


                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", baseDomain),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "1.0")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.9")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/about-us"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.9")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/contact-us"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.9")));

                
                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/business"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/digital-media-and-information-technology"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));
                
                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/early-childhood-education-and-care"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/english"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/environment-management-and-sustainability"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/hospitality-and-culinary"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/leadership-and-management"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/marine-habitat-conservation-and-restoration"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/qualifications/marketing-and-communications"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/faq"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.8")));

                sitemapHead.Add(new XElement(blank + "url",
                    new XElement(blank + "loc", $"{baseDomain}/news-and-events"),
                    new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                    new XElement(blank + "priority", "0.6")));

                foreach (var article in _articles)
                {
                    sitemapHead.Add(new XElement(blank + "url",
                        new XElement(blank + "loc", $"{baseDomain}/news-and-events/{article.Route}"),
                        new XElement(blank + "lastmod", String.Format("{0}", _lastModifiedDate.ToString("O"))),
                        new XElement(blank + "changefreq", "monthly"),
                        new XElement(blank + "priority", "0.5")));
                }
                document.Add(sitemapHead);
                document.Save(writer);
            }
        }
    }
}
