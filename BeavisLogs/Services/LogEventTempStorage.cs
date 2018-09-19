using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisCli;
using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using Microsoft.Extensions.Caching.Memory;

namespace BeavisLogs.Services
{
    

    public class LogEventTempStorage
    {
        private readonly IMemoryCache _memoryCache;

        public LogEventTempStorage(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        public LogEventSlot CreateSlot(DataSourceInfo source)
        {
            var slot = new LogEventSlot();

            string key = KeyUtil.GenerateKey();

                     
            return slot;
        }

        private bool TrySetSlot(LogEventSlot slot)
        {
            return true;
        }

        private bool TryGetSlot(string key, out LogEventSlot slot)
        {
            slot = null;
            return true;
        }

        private bool TryRemoveSlot(string key, out LogEventSlot slot)
        {
            slot = null;
            return true;
        }
    }

   
}
