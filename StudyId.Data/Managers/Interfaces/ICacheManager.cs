using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.Data.Managers.Interfaces
{
    public interface ICacheManager
    {
        T? CacheGetValueOrNull<T>(string key);
        void CacheSetValue<T>(string key, T item, int expireInMinutes=3);
        void PurgeCache(string key);
    }
}
