using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using System;

namespace BeavisLogs.Drivers
{
    public sealed class QueryContext
    {
        private readonly LogEventSlot _slot;
        private readonly DataSourceInfo _source;

        public QueryContext(LogEventSlot slot, DataSourceInfo source, DriverProperties driverProperties)
        {          
            _slot = slot;
            _source = source;
            DriverProperties = driverProperties;
        }
       
        /// <summary>
        /// Parameters for the query
        /// </summary>
        public QueryParameters Parameters { get; } = new QueryParameters();

        /// <summary>
        /// Driver properties
        /// </summary>
        public DriverProperties DriverProperties {get;}

        /// <summary>
        /// Occurs when query has been started.
        /// </summary>
        public void OnQueryStarted()
        {
            // not yet implemented
        }

        /// <summary>
        /// Occurs when found log events.
        /// </summary>
        public void OnFound(params ILogEvent[] events)
        {
            _slot.OnFound(_source, events);
        }

        /// <summary>
        /// Occurs when an error has been occurred.
        /// </summary>
        public void OnErrorOccurred(Exception ex)
        {
            _slot.OnErrorOccurred(_source, ex);
        }

        /// <summary>
        /// Occurs when query has been completed.
        /// </summary>
        public void OnQueryCompleted()
        {
            _slot.OnQueryCompleted(_source);
        }

        /// <summary>
        /// Checks if the query is still alive.
        /// </summary>
        public bool IsAlive()
        {
            return !_slot.IsLimitReached && !_slot.IsCompleted(_source);
        }
    }
}
