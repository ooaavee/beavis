using BeavisLogs.Commands.See;
using BeavisLogs.Models.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisLogs.Services
{
    public class LogEventRenderer
    {
        public string Render(ILogEvent e, bool detailed = false)
        {
            string timestamp = FormatTimestamp(e.Timestamp);
            string level = LogLevelUtil.GetLevelText(e.Level);
            string body = FormatBody(e.Message, e.Exception, e.Properties, detailed);
            string text = $"{timestamp} [{level}] {body}";
            return text;
        }

        private string FormatTimestamp(DateTimeOffset timestamp)
        {
            DateTime time;

            TimeZoneInfo zone = GetCustomOutputTimeZone();

            if (zone == null)
            {
                time = timestamp.UtcDateTime;
            }
            else
            {
                time = TimeZoneInfo.ConvertTimeFromUtc(timestamp.UtcDateTime, zone);
            }

            string s = time.ToString("yyyy-MM-ddTHH\\:mm\\:sszzz");      
            return s;
        }

        private TimeZoneInfo _customOutputTimeZone;

        private TimeZoneInfo GetCustomOutputTimeZone()
        {
            // TODO: read output time-zone from user's options
            string zoneId = "FLE Standard Time";

            if (_customOutputTimeZone == null)
            {
                _customOutputTimeZone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            }

            //try
            //{
            //    _customZone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            //}
            //catch (TimeZoneNotFoundException)
            //{
            //    Console.WriteLine("The registry does not define the Central Standard Time zone.");
            //}
            //catch (InvalidTimeZoneException)
            //{
            //    Console.WriteLine("Registry data on the Central Standard Time zone has been corrupted.");
            //}

            return _customOutputTimeZone;
        }

        private string FormatBody(string message, string exception, Dictionary<string, object> properties, bool detailed)
        {
            StringBuilder text = new StringBuilder();

            bool hasMessage = !string.IsNullOrEmpty(message);
            bool hasException = !string.IsNullOrEmpty(exception);

            if (hasMessage)
            {
                text.Append(message);
            }

            if (hasException)
            {
                if (hasMessage)
                {
                    text.Append(Environment.NewLine);
                }

                text.Append(exception);
            }

            return text.ToString();
        }
    }
}
