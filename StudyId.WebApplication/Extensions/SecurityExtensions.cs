using System.Security.Claims;
using System.Security.Principal;

namespace StudyId.WebApplication.Extensions
{
    public static class SecurityExtensions
    {
        private const string schema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";
        private const string newschema = "http://schemas.microsoft.com/ws/2008/06/identity/claims/";
        public static string? GetClaimUserId(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return null;
            var dbFiled = identity as ClaimsIdentity;
            var dbProp = dbFiled?.Claims.FirstOrDefault(x => x.Type.ToLower().EndsWith("nameidentifier"));
            return dbProp?.Value;
        }
        public static string? GetClaimUserEmail(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return null;
            var dbFiled = identity as ClaimsIdentity;
            var dbProp = dbFiled?.Claims.FirstOrDefault(x => x.Type.ToLower().EndsWith("email"));
            return dbProp?.Value;
        }
        public static string? GetClaimUserFullName(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return null;
            var dbFiled = identity as ClaimsIdentity;
            var dbProp = dbFiled?.Claims.FirstOrDefault(x => x.Type.ToLower() == schema + "fullname");
            return dbProp?.Value;
        }

        public static string? GetClaimRole(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return null;
            var dbFiled = identity as ClaimsIdentity;
            var dbProp = dbFiled?.Claims.FirstOrDefault(x => x.Type.ToLower() == newschema + "role");
            return dbProp?.Value;
        }
    }
}
