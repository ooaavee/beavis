//using System;
//using System.Globalization;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.CommandLineUtils;

//namespace Jemma.Terminal.Applications.Search
//{
//    internal class Search : AbstractApplication
//    {
//        public static readonly BeavisApplicationInfo Definition = new BeavisApplicationInfo
//        {
//            Type = typeof(Search),
//            Name = "search",
//            Description = "Search log events.",
//            AllowAnonymous = true
//        };

//        protected override async Task OnRunAsync(TerminalExecutionContext context)
//        {
//            var app = CreateApplication(Definition, context);

//            // Options
//            CommandOption after = app.Option("-after", "Search log events occurred after, ISO 8601 date time or time interval relative to the current time.", CommandOptionType.SingleValue);
//            CommandOption before = app.Option("-before", "Search log events occurred before, ISO 8601 date time or time interval relative to the current time. Optional.", CommandOptionType.SingleValue);
//            CommandOption level = app.Option("-level", "Search log events with a level or higher (supported values are: " + LogLevelText.GetValuesText() + ").", CommandOptionType.SingleValue);

//            // TODO:
//            // -grep 

//            await app.OnExecuteAsync(() =>
//            {
//                // Required option: -after
//                DateTime? afterDate;
//                if (HasOption(after))
//                {
//                    string afterStr = GetValue(after);
//                    if (!TryParseDateTimeOption(afterStr, context.TimestampUtc, out afterDate))
//                    {
//                        WriteDateTimeOptionError(after.Template, afterStr, context);
//                        return Exit();
//                    }
//                }
//                else
//                {
//                    app.ShowHelp(Definition.Name);
//                    return Exit();
//                }

//                // Optional option: -before
//                DateTime? beforeDate = null;
//                if (HasOption(before))
//                {
//                    string beforeStr = GetValue(before);
//                    if (!TryParseDateTimeOption(beforeStr, context.TimestampUtc, out beforeDate))
//                    {
//                        WriteDateTimeOptionError(before.Template, beforeStr, context);
//                        return Exit();
//                    }
//                }

//                if (beforeDate != null)
//                {
//                    if (!(afterDate <= beforeDate))
//                    {
//                        using (context.Response.BeginInteraction())
//                        {
//                            context.Response.WriteError("dasdasd");
//                        }
//                    }
//                }


//                // Optional option: -level
//                LogLevel? levelEnum = null;
//                bool hasLevel = HasOption(level);
//                if (hasLevel)
//                {
//                    string levelStr = GetValue(level);
//                    if (!LogLevelText.TryParseLogLevel(levelStr, out levelEnum))
//                    {
//                        WriteLevelOptionError(level.Template, levelStr, context);
//                        return Exit();
//                    }
//                }



//                // TODO:


//                // - tarkista että after < before




//                // Required services
//                TerminalPollCache cache = context.Service<TerminalPollCache>();
//                ILogReader reader = context.Service<ILogReader>();

//                // Cache key
//                string key = cache.GetOrCreateSlotKey(context);

//                LogReaderOptions options = new LogReaderOptions
//                {
//                    After = afterDate,
//                    Before = beforeDate,
//                    Level = levelEnum,
//                    ContinueSearch = () =>
//                    {
//                        if (cache.IsSlotAvailable(key))
//                        {
//                            return true;
//                        }

//                        // If cache slot is not available --> do not continue search!
//                        return false;
//                    },
//                    OnLogEntriesFound = entries =>
//                    {
//                        if (cache.TryGetSlot(key, out TerminalPollCacheSlot slot))
//                        {
//                            slot.Add(entries);
//                        }
//                    }
//                };

//                // Execute search in a new thread!
//                Thread t = new Thread(() => { reader.ReadAsync(options); });
//                t.IsBackground = true;
//                t.Start();

//                return Exit();
//            });

//            Execute(app, context);
//        }

//        private static bool TryParseDateTimeOption(string iso8601OrTimeInterval, DateTime utcNow, out DateTime? date)
//        {
//            date = null;

//            if (iso8601OrTimeInterval.StartsWith("t-"))
//            {
//                string tmp = iso8601OrTimeInterval.Remove(0, 2);
//                try
//                {
//                    TimeSpan interval = TimeSpan.Parse(tmp, CultureInfo.InvariantCulture);
//                    date = utcNow.Subtract(interval);
//                    return true;
//                }
//                catch (Exception)
//                {
//                    return false;
//                }
//            }

//            try
//            {
////                DateTime tmp = DateTime.Parse(iso8601OrTimeInterval, null, DateTimeStyles.RoundtripKind);
//                DateTime tmp = DateTime.Parse(iso8601OrTimeInterval, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
//                if (tmp.Kind != DateTimeKind.Utc)
//                {
//                    tmp = tmp.ToUniversalTime();
//                }

//                date = tmp;
//                return true;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        private static void WriteDateTimeOptionError(string option, string optionValue, TerminalExecutionContext context)
//        {
//            using (context.Response.BeginInteraction())
//            {
//                context.Response.WriteError(string.Format("{0} is not a valid value for the option '{1}'.", optionValue, option));
//                context.Response.WriteError("Please enter a valid ISO 8601 date time or a time interval relative to the current time.");
//                context.Response.WriteEmptyLine();
//                context.Response.WriteError("Examples for ISO 8601 date time:");
//                context.Response.WriteError(" " + context.TimestampUtc.ToString("s"));

//                // TODO: Tässä voisi näyttää linkin johonkin speksiin, jossa näkyy tuo formaatti

//                context.Response.WriteEmptyLine();
//                context.Response.WriteError("Examples for time intervals:");
//                context.Response.WriteError(" t-1            1 day ago");
//                context.Response.WriteError(" t-1.0:15       1 day 15 minutes ago");
//                context.Response.WriteError(" t-1.6:15       1 day 6 hours 15 minutes ago");
//                context.Response.WriteError(" t-0:15         15 minutes ago");
//                context.Response.WriteError(" t-1:30         1 hour 30 minutes ago");
//                context.Response.WriteError(" t-1:30:55      1 hour 30 minutes 55 seconds ago");

//                // TODO: Tässä voisi näyttää linkin johonkin speksiin, jossa näkyy tuo formaatti
//            }
//        }

//        private static void WriteLevelOptionError(string option, string optionValue, TerminalExecutionContext context)
//        {
//            using (context.Response.BeginInteraction())
//            {
//                context.Response.WriteError(string.Format("{0} is not a valid value for the option '{1}'", optionValue, option));
//                context.Response.WriteError("Supported values are: " + LogLevelText.GetValuesText());
//            }
//        }
//    }
//}
