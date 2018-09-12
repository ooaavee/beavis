using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace BeavisLogs.Drivers
{
    public sealed class QueryParameters
    {

        public QueryParameters()
        {
            //Levels.Add(LogLevel.Error);
            //Levels.Add(LogLevel.Critical);

            Regex = "(?i)hinweggeschwunden die die folgt läng";

        }

        public List<LogLevel> Levels { get; } = new List<LogLevel>();

        public string Regex { get; set; }

    }
}