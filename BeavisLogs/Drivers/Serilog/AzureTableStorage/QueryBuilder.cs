using BeavisLogs.Models.Logs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class QueryBuilder
    {
        private readonly LogEventMapper _mapper;

        public QueryBuilder(LogEventMapper mapper)
        {
            _mapper = mapper;
        }

        public Query Build(QueryContext context)
        {
            return new Query(
                CreateTableQuery(context.Parameters),
                CreateFilters(context.Parameters)
            );
        }

        private TableQuery<LogEventTableEntity> CreateTableQuery(QueryParameters p)
        {
            string filter;
            string partitionKeyFrom = _mapper.GetPartitionKey(p.From);

            if (p.Until != null)
            {
                string partitionKeyUntil = _mapper.GetPartitionKey(p.Until.GetValueOrDefault());
                string filterFrom = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, partitionKeyFrom);
                string filterUntil = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThanOrEqual, partitionKeyUntil);
                filter = TableQuery.CombineFilters(filterFrom, TableOperators.And, filterUntil);
            }
            else
            {
                filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, partitionKeyFrom);
            }

            return new TableQuery<LogEventTableEntity>().Where(filter);
        }

        private IEnumerable<Predicate<ILogEvent>> CreateFilters(QueryParameters p)
        {
            if (p.Levels.Any())
            {
                yield return LogLevelFilter(p.Levels);
            }

            if (p.MessageAndExceptionPattern != null)
            {
                yield return MessageAndExceptionPatternFilter(p.MessageAndExceptionPattern);
            }

            if (p.MessageAndExceptionText != null)
            {
                yield return MessageAndExceptionTextFilter(p.MessageAndExceptionText);
            }

            foreach (var item in p.PropertyPattern)
            {
                yield return PropertyPatternFilter(item.Key, item.Value);
            }

            foreach (var item in p.PropertyText)
            {
                yield return PropertyTextFilter(item.Key, item.Value);
            }

            if (p.Until != null)
            {
                yield return TimestampFilter(p.From, p.Until.Value);
            }
            else
            {
                yield return TimestampFilter(p.From);
            }
        }

        /// <summary>
        /// Creates a filter for <see cref="ILogEvent.Level"/> property.
        /// </summary>
        private static Predicate<ILogEvent> LogLevelFilter(IEnumerable<LogLevel> levels)
        {
            return e => levels.Contains(e.Level);
        }

        /// <summary>
        /// Creates a filter for <see cref="ILogEvent.Message"/> and <see cref="ILogEvent.Exception"/> properties.
        /// </summary>
        private static Predicate<ILogEvent> MessageAndExceptionPatternFilter(string pattern)
        {
            return e => PatternMatch(e.Message, pattern) || PatternMatch(e.Exception, pattern);
        }

        /// <summary>
        /// Creates a filter for <see cref="ILogEvent.Message"/> and <see cref="ILogEvent.Exception"/> properties.
        /// </summary>
        private static Predicate<ILogEvent> MessageAndExceptionTextFilter(string text)
        {
            return e => TextMatch(e.Message, text) || TextMatch(e.Exception, text);
        }

        /// <summary>
        /// Creates a filter for 'named property'.
        /// </summary>
        private static Predicate<ILogEvent> PropertyPatternFilter(string key, string pattern)
        {
            return e => e.Properties.TryGetValue(key, out var value) && PatternMatch(value, pattern);
        }

        /// <summary>
        /// Creates a filter for 'named property'.
        /// </summary>
        private static Predicate<ILogEvent> PropertyTextFilter(string key, string text)
        {
            return e => e.Properties.TryGetValue(key, out var value) && TextMatch(value, text);
        }

        private static Predicate<ILogEvent> TimestampFilter(DateTimeOffset from, DateTimeOffset until)
        {
            return e => from.UtcTicks <= e.Timestamp.UtcTicks && until.UtcTicks >= e.Timestamp.UtcTicks;
        }

        private static Predicate<ILogEvent> TimestampFilter(DateTimeOffset from)
        {
            return e => from.UtcTicks <= e.Timestamp.UtcTicks;
        }

        private static bool PatternMatch(string input, string pattern)
        {
            return input != null && Regex.IsMatch(input, pattern);
        }

        private static bool PatternMatch(object input, string pattern)
        {
            return input != null && PatternMatch(input.ToString(), pattern);
        }

        private static bool TextMatch(string input, string text)
        {
            return input != null && input.Contains(text, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool TextMatch(object input, string text)
        {
            return input != null && TextMatch(input.ToString(), text);
        }
    }
}
