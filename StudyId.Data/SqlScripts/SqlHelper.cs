using System.Reflection;

namespace StudyId.Data.SqlScripts
{
    public static class SqlHelper
    {
        public static string ReadScript(string name)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "StudyId.Data.SqlScripts." + name;
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }
    
}
}



