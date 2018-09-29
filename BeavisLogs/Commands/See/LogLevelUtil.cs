using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisLogs.Commands.See
{
    public static class LogLevelUtil
    {
        private static readonly List<Tuple<LogLevel, string>> All = new List<Tuple<LogLevel, string>>();
        private static readonly Dictionary<LogLevel, string> Map = new Dictionary<LogLevel, string>();

        static LogLevelUtil()
        {
            void Use(LogLevel level, string text)
            {
                All.Add(new Tuple<LogLevel, string>(level, text));
                Map.Add(level, text);
            }

            Use(LogLevel.Trace, "TRCE");
            Use(LogLevel.Debug, "DBUG");
            Use(LogLevel.Information, "INFO");
            Use(LogLevel.Warning, "WARN");
            Use(LogLevel.Error, "FAIL");
            Use(LogLevel.Critical, "CRIT");
        }

        public static string GetLevelText(LogLevel level)
        {
            if (Map.TryGetValue(level, out var text))
            {
                return text;
            }

            if (level == LogLevel.None)
            {
                return "none";
            }

            throw new ArgumentOutOfRangeException(nameof(level));
        }

        public static string GetLevelTexts()
        {
            var buf = new StringBuilder();

            for (var i = 0; i < All.Count; i++)
            {
                var logLevel = All[i];

                var text = logLevel.Item2;

                if (i != All.Count - 1)
                {
                    if (i > 0)
                    {
                        buf.Append(", ");
                    }
                }
                else
                {
                    buf.Append(" or ");
                }

                buf.Append(text);
            }

            return buf.ToString();
        }
    }
}
