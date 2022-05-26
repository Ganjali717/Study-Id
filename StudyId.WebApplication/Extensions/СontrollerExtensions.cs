using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace StudyId.WebApplication.Extensions
{
    public static class СontrollerExtensions
    {
        public static HtmlString ToJson(this IHtmlHelper htmlHelper, object obj, bool addQuotes = false,
            bool encode = true)
        {
            if (obj == null) return new HtmlString(string.Empty);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            if (addQuotes)
            {
                json = string.Concat("'", json, "'");
            }

            if (!encode)
            {
                json = htmlHelper.Encode(json);
            }

            return new HtmlString(json);
        }
        public static string GetVersion(this IHtmlHelper htmlHelper)
        {
            return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }

   

}
