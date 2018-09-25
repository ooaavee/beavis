using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisCli;
using BeavisLogs.Models.Logs;

namespace BeavisLogs.Drivers
{
    public class LogEventSlot
    {
        private readonly object _lock = new object();

        private readonly List<ILogEvent> _events = new List<ILogEvent>();
        private readonly List<Exception> _exceptions = new List<Exception>();

        public bool IsCompleted { get; private set; }

        public LogEventSlot()
        {
            Key = KeyUtil.GenerateKey();
        }


        public string Key { get; }

        public void OnFound(ILogEvent[] events)
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("Already completed.");
            }

            lock (_lock)
            {
                _events.AddRange(events);
            }
        }

        public void OnErrorOccurred(Exception ex)
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("Already completed.");
            }

            lock (_lock)
            {
                _exceptions.Add(ex);
            }
        }

        public void OnQueryCompleted()
        {
            lock (_lock)
            {
                IsCompleted = true;
            }
        }







    }
}
