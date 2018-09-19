using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisLogs.Models.Logs;

namespace BeavisLogs.Drivers
{
    public class LogEventSlot
    {
        private bool _completed = false;
        private readonly ConcurrentQueue<ILogEvent> _events = new ConcurrentQueue<ILogEvent>();
        private readonly ConcurrentQueue<DriverException> _exceptions = new ConcurrentQueue<DriverException>();

        public void OnFound(ILogEvent[] events)
        {
            if (_completed)
            {
                throw new InvalidOperationException("Already completed.");
            }

            foreach (var e in events)
            {
                _events.Enqueue(e);
            }
        }

        public void OnErrorOccurred(DriverException exception)
        {
            if (_completed)
            {
                throw new InvalidOperationException("Already completed.");
            }

            _exceptions.Enqueue(exception);
        }

        public void OnQueryCompleted()
        {
            if (_completed)
            {
                throw new InvalidOperationException("Already completed.");
            }

            _completed = true;
        }






    }
}
