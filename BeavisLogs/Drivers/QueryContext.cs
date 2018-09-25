using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using System;

namespace BeavisLogs.Drivers
{
    public sealed class QueryContext
    {
        private readonly LogEventSlot _slot;
        private readonly DataSource _source;

        public QueryContext(LogEventSlot slot, DataSource source)
        {
            _slot = slot;
            _source = source;
        }
       
        /// <summary>
        /// Parameters for the query
        /// </summary>
        public QueryParameters Parameters { get; } = new QueryParameters();

        /// <summary>
        /// Driver properties
        /// </summary>
        public DriverProperties DriverProperties => _source.DriverProperties;

        public void OnFound(params ILogEvent[] events)
        {
            _slot.OnFound(events);
        }

        public void OnQueryCompleted()
        {
            _slot.OnQueryCompleted();
        }

        public void OnErrorOccurred(Exception ex)
        {
            _slot.OnErrorOccurred(ex);
        }

        public bool IsAlive()
        {
            return !_slot.IsCompleted;
        }
    }
}
