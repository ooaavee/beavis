using System;
using System.Linq;
using System.Threading.Tasks;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;

namespace BeavisLogs.Drivers
{
    public sealed class QueryContext
    {
        public QueryContext(LogEventSlot slot, DriverProperties driverProperties)
        {
            Slot = slot;
            DriverProperties = driverProperties;
        }

        private LogEventSlot Slot { get; }

        /// <summary>
        /// Parameters for the query
        /// </summary>
        public QueryParameters Parameters { get; } = new QueryParameters();

        /// <summary>
        /// Driver properties
        /// </summary>
        public DriverProperties DriverProperties { get; }

        public void OnFound(params ILogEvent[] events)
        {
            Slot.OnFound(events);
        }

        public void OnQueryCompleted()
        {
            Slot.OnQueryCompleted();
        }

        public void OnErrorOccurred(DriverException exception)
        {
            Slot.OnErrorOccurred(exception);
        }

        public bool IsAlive()
        {
            return false;
        }        
    }
}
