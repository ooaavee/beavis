using BeavisLogs.Models.Logs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class QueryFilterBuilder
    {
        private readonly LogEventMapper _mapper;

        public QueryFilterBuilder(LogEventMapper mapper)
        {
            _mapper = mapper;
        }

        public QueryFilter Build(QueryContext context)
        {
            return new QueryFilter(Query(context.Parameters), Filters(context.Parameters));
        }

        private TableQuery<LogEventTableEntity> Query(QueryParameters parameters)
        {
            string partitionKeyFrom = _mapper.GetPartitionKey(parameters.From);
            string partitionKeyUntil = _mapper.GetPartitionKey(parameters.Until);

            string filterFrom = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, partitionKeyFrom);
            string filterUntil = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThanOrEqual, partitionKeyUntil);
            string filter = TableQuery.CombineFilters(filterFrom, TableOperators.And, filterUntil);
            
            return new TableQuery<LogEventTableEntity>().Where(filter);
        }

        private IEnumerable<Predicate<ILogEvent>> Filters(QueryParameters parameters)
        {
            if (parameters.Levels.Any())
            {
                yield return LogLevelFilter(parameters.Levels);
            }

            if (parameters.MessageAndExceptionPattern != null)
            {
                yield return MessageAndExceptionFilter(parameters.MessageAndExceptionPattern);
            }

            if (parameters.PropertyPattern.Any())
            {
                foreach (KeyValuePair<string, string> pp in parameters.PropertyPattern)
                {
                    yield return PropertyFilter(pp.Key, pp.Value);
                }
            }

            yield return TimestampFilter(parameters.From, parameters.Until);
        }

        /// <summary>
        /// Creates a filter for <see cref="ILogEvent.Level"/> property.
        /// </summary>
        private static Predicate<ILogEvent> LogLevelFilter(IEnumerable<LogLevel> levels)
        {
            return delegate (ILogEvent e)
            {
                return levels.Contains(e.Level);
            };
        }

        /// <summary>
        /// Creates a filter for <see cref="ILogEvent.Message"/> and <see cref="ILogEvent.Exception"/> properties.
        /// </summary>
        private static Predicate<ILogEvent> MessageAndExceptionFilter(string pattern)
        {
            return delegate (ILogEvent e)
            {
                if (Match(e.Message, pattern))
                {
                    return true;
                }
                if (Match(e.Exception, pattern))
                {
                    return true;
                }                
                return false;
            };
        }

        /// <summary>
        /// Creates a filter for 'named property'.
        /// </summary>
        private static Predicate<ILogEvent> PropertyFilter(string key, string pattern)
        {
            return delegate (ILogEvent e)
            {
                if (e.Properties.TryGetValue(key, out object value))
                {
                    if (Match(value, pattern))
                    {
                        return true;
                    }
                }
                return false;
            };
        }

        private static Predicate<ILogEvent> TimestampFilter(DateTimeOffset from, DateTimeOffset until)
        {
            return delegate (ILogEvent e)
            {
                long ticks = e.Timestamp.UtcTicks;
                if (from.UtcTicks <= ticks && until.UtcTicks >= ticks)
                {
                    return true;
                }
                return false;
            };
        }

        private static bool Match(string input, string pattern)
        {
            if (input != null)
            {
                if (Regex.IsMatch(input, pattern))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Match(object input, string pattern)
        {
            if (input != null)
            {
                return Match(input.ToString(), pattern);
            }
            return false;
        }
    }
}
