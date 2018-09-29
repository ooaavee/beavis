using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace BeavisLogs.Services
{
    public class LogEventTempStorage
    {
        private readonly IMemoryCache _memoryCache;

        public LogEventTempStorage(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public LogEventSlot CreateSlot(IEnumerable<DataSourceInfo> sources)
        {
            var slot = new LogEventSlot(sources, 1000);
            SetSlot(slot);
            return slot;
        }

        private void SetSlot(LogEventSlot slot)
        {
            var cacheKey = GetMemoryCacheKey(slot.Key);
            var options = new MemoryCacheEntryOptions {Priority = CacheItemPriority.High, SlidingExpiration = TimeSpan.FromMinutes(3)};
            _memoryCache.Set(cacheKey, slot, options);
        }

        public bool TryGetSlot(string key, out LogEventSlot slot)
        {
            var cacheKey = GetMemoryCacheKey(key);
            return _memoryCache.TryGetValue(cacheKey, out slot);
        }

        public bool TryRemoveSlot(string key, out LogEventSlot slot)
        {
            var cacheKey = GetMemoryCacheKey(key);
            if (_memoryCache.TryGetValue(cacheKey, out slot))
            {
                _memoryCache.Remove(cacheKey);
                return true;
            }
            slot = null;
            return false;
        }

        private static string GetMemoryCacheKey(string key)
        {
            return $"__BeavisLogs.Services.LogEventTempStorage.{key}";
        }
    }   
}
