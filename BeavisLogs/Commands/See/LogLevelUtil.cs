using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BeavisLogs.Commands.See
{
    public static class LogLevelUtil
    {
        private static readonly List<Tuple<LogLevel, string>> Map = new List<Tuple<LogLevel, string>>(
            new[]
            {
                new Tuple<LogLevel, string>(LogLevel.Trace, "trce"),
                new Tuple<LogLevel, string>(LogLevel.Debug, "dbug"),
                new Tuple<LogLevel, string>(LogLevel.Information, "info"),
                new Tuple<LogLevel, string>(LogLevel.Warning, "warn"),
                new Tuple<LogLevel, string>(LogLevel.Error, "fail"),
                new Tuple<LogLevel, string>(LogLevel.Critical, "crit")
            });

        public static string GetValuesText()
        {
            var buf = new StringBuilder();

            for (var i = 0; i < Map.Count; i++)
            {
                var logLevel = Map[i];

                var text = logLevel.Item2;

                if (i != Map.Count - 1)
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
