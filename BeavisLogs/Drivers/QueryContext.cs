using System;
using System.Linq;
using System.Threading.Tasks;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;

namespace BeavisLogs.Drivers
{
    public delegate void LogEventsFoundEventHandler(params ILogEvent[] events);
    public delegate void LogEventQueryCompletedEventHandler();
    public delegate void LogEventQueryErrorOccurredEventHandler(DriverException e);

    public sealed class QueryContext
    {
        public event LogEventsFoundEventHandler Found;
        public event LogEventQueryCompletedEventHandler QueryCompleted;
        public event LogEventQueryErrorOccurredEventHandler ErrorOccurred;

        public DriverProperties DriverProperties { get; set; }

        public QueryParameters Parameters { get; } = new QueryParameters();

        public void OnFound(params ILogEvent[] events)
        {
            Found?.Invoke(events);
        }

        public void OnQueryCompleted()
        {
            QueryCompleted?.Invoke();
        }

        public void OnErrorOccurred(DriverException e)
        {
            ErrorOccurred?.Invoke(e);
        }

        public bool IsCanceled()
        {
            return false;
        }


        
    }
}
