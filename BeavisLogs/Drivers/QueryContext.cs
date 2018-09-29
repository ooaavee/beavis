using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using System;
using BeavisLogs.Services;

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
            _slot.OnFound(_source.Info, events);
        }

        public void OnQueryCompleted()
        {
            _slot.OnQueryCompleted(_source.Info);
        }

        public void OnErrorOccurred(Exception ex)
        {
            _slot.OnErrorOccurred(_source.Info, ex);
        }

        public bool IsAlive()
        {
            if (_slot.IsLimitReached)
            {
                return false;
            }

            if (_slot.IsCompleted(_source.Info))
            {
                return false;
            }

            return true;
        }
    }
}
