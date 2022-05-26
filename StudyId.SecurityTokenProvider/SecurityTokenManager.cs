using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.SecurityTokenProvider
{
    public class SecurityTokenManager : ISecurityTokenManager
    {
        public string GenerateTimeStampToken(DateTime date)
        {
            var dayTime = BitConverter.GetBytes(date.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(dayTime.Concat(key).ToArray());
        }

        public DateTime GetTimeStampToken(string token)
        {
            var data = Convert.FromBase64String(token);
            return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        }
    }
}
