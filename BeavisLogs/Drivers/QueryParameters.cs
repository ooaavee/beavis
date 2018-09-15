using System;
using System.Collections.Generic;
using BeavisLogs.Models.Logs;
using Microsoft.Extensions.Logging;

namespace BeavisLogs.Drivers
{
    public sealed class QueryParameters
    {
        public QueryParameters()
        {
            Levels.Add(LogLevel.Error);
            Levels.Add(LogLevel.Critical);

            //            MessageAndExceptionPattern = "(?i)hinweggeschwunden die die folgt läng";

            PropertyPattern.Add("Exception", "DataException");


            From = DateTimeOffset.Now.AddDays(-100);
            Until = DateTimeOffset.Now;

        }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset Until { get; set; }

        /// <summary>
        /// Filter for <see cref="ILogEvent.Level"/>.
        /// </summary>
        public List<LogLevel> Levels { get; } = new List<LogLevel>();

        /// <summary>
        /// Filter for <see cref="ILogEvent.Message"/> and <see cref="ILogEvent.Exception"/> by using regular expressions.
        /// </summary>
        public string MessageAndExceptionPattern { get; set; }

        /// <summary>
        /// Filter for <see cref="ILogEvent.Properties"/> by using regular expressions.
        /// </summary>
        public Dictionary<string, string> PropertyPattern { get; } = new Dictionary<string, string>();
    }
}