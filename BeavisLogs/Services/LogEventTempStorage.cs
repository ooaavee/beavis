using BeavisLogs.Drivers;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BeavisLogs.Services
{
    public class LogEventTempStorage
    {
        private readonly IMemoryCache _memoryCache;

        private readonly MemoryCacheEntryOptions _entryOptions = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromMinutes(3)
        };

        public LogEventTempStorage(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set(LogEventSlot slot)
        {
            string cacheKey = GetMemoryCacheKey(slot.Properties.Key);          
            _memoryCache.Set(cacheKey, slot, _entryOptions);
        }
 
        public bool TryGet(string key, out LogEventSlot slot)
        {
            string cacheKey = GetMemoryCacheKey(key);
            return _memoryCache.TryGetValue(cacheKey, out slot);
        }

        public bool Remove(string key)
        {
            string cacheKey = GetMemoryCacheKey(key);
            if (_memoryCache.TryGetValue(cacheKey, out _))
            {
                _memoryCache.Remove(cacheKey);
                return true;
            }
            return false;
        }

        private static string GetMemoryCacheKey(string key)
        {
            return $"__BeavisLogs.Services.LogEventTempStorage.{key}";
        }
    }   
}
