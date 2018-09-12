using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BeavisLogs.Models.Logs
{
    public interface ILogEvent
    {
        DateTimeOffset Timestamp { get; }

        LogLevel Level { get; }

        string Message { get; }

        string Exception { get; }

        Dictionary<string, object> Properties { get; }

        void ReadLogEvent(IDictionary<string, object> values);
    }
}
