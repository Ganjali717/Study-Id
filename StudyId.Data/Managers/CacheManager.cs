using Microsoft.Extensions.Caching.Memory;
using StudyId.Data.Managers.Interfaces;

namespace StudyId.Data.Managers
{
    public class CacheManager:ICacheManager
    {
        private readonly IMemoryCache  _memoryCache;

        public CacheManager(IMemoryCache  memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? CacheGetValueOrNull<T>(string key)
        {
            var data = _memoryCache.Get<T>(key);
            return data ?? default(T);
        }

        public void CacheSetValue<T>(string key, T item, int expireInMinutes = 3)
        {
            if (key == null) throw new ArgumentNullException(key);
            _memoryCache.Set(key, item, TimeSpan.FromMinutes(expireInMinutes));
        }

        public void PurgeCache(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
