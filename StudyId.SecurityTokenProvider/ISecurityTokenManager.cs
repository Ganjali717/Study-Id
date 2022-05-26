using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.SecurityTokenProvider
{
    /// <summary>
    /// Provide the operations for generate uniq security token
    /// </summary>
    public interface ISecurityTokenManager
    {
        /// <summary>
        /// Generate token from the date
        /// </summary>
        /// <param name="date">Datetime stamp</param>
        /// <returns></returns>
        string GenerateTimeStampToken(DateTime date);
        /// <summary>
        /// Get the date time from the token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        DateTime GetTimeStampToken(string token);
    }
}
