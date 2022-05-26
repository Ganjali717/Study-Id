using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace StudyId.Entities.Extentions
{
    public static class StringExtentions
    {
        public static string GetSHA256Hash(this string item)
        {
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(item));
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
        public static string GetSHA1Hash(this string item)
        {
            var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(item));
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
        public static long GetIntHash(this string item)
        {
            var temp = GetSHA1Hash(item);
            var regex = new Regex(@"[^\d]");
            var onlyDigits = regex.Replace(temp, string.Empty);
            if (onlyDigits.Length >= 19) onlyDigits = onlyDigits.Substring(0, 18);
            return Int64.Parse(onlyDigits);
            
        }
    }
}
