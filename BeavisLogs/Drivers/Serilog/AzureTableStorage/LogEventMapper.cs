using BeavisLogs.Models.Logs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog.Events;
using Serilog.Sinks.AzureTableStorage.KeyGenerator;
using System;
using System.Collections.Generic;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class LogEventMapper
    {
        // Serilog's default key generator
        private static readonly IKeyGenerator KeyGenerator = new DefaultKeyGenerator();

        public bool TryMap(LogEventTableEntity entity, LogEventMappingContext mappingContext, out ILogEvent e)
        {
            e = null;

            // Timestamp
            if (!TryParseTimestamp(entity, mappingContext, out DateTimeOffset timestamp))
            {
                return false;
            }

            // Level
            LogLevel level;
            if (entity.TryGetValue("Level", out EntityProperty levelProperty) && levelProperty.PropertyType == EdmType.String) 
            {
                switch (levelProperty.StringValue)
                {
                    case "Verbose":
                        level = LogLevel.Trace;
                        break;
                    case "Debug":
                        level = LogLevel.Debug;
                        break;
                    case "Information":
                        level = LogLevel.Information;
                        break;
                    case "Warning":
                        level = LogLevel.Warning;
                        break;
                    case "Error":
                        level = LogLevel.Error;
                        break;
                    case "Fatal":
                        level = LogLevel.Critical;
                        break;
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }

            // Message
            string message;
            if (entity.TryGetValue("RenderedMessage", out EntityProperty renderedMessageProperty) && renderedMessageProperty.PropertyType == EdmType.String)
            {
                message = renderedMessageProperty.StringValue;
            }
            else
            {
                message = null;
            }

            // Exception
            string exception;
            if (entity.TryGetValue("Exception", out EntityProperty exceptionProperty) && exceptionProperty.PropertyType == EdmType.String)
            {
                exception = exceptionProperty.StringValue;
            }
            else
            {
                exception = null;
            }

            // Properties
            var properties = new Dictionary<string, object>
            {
                {nameof(entity.PartitionKey), entity.PartitionKey},
                {nameof(entity.RowKey), entity.RowKey},
                {nameof(entity.Timestamp), entity.Timestamp},
                {nameof(entity.ETag), entity.ETag}
            };
            foreach (var property in entity)
            {
                properties.Add(property.Key, property.Value.PropertyAsObject);
            }

            var values = new Dictionary<string, object>
            {
                [nameof(ILogEvent.Timestamp)] = timestamp,
                [nameof(ILogEvent.Level)] = level,
                [nameof(ILogEvent.Message)] = message,
                [nameof(ILogEvent.Exception)] = exception,
                [nameof(ILogEvent.Properties)] = properties
            };
                  
            e = entity;           
            e.ReadLogEvent(values);

            return true;
        }
        
        private bool TryParseTimestamp(LogEventTableEntity entity, LogEventMappingContext mappingContext, out DateTimeOffset timestamp)
        {
            if (long.TryParse(entity.PartitionKey, out long ticks) && ticks > 0)
            {
                DateTime timeWithoutMilliseconds = new DateTime(ticks, DateTimeKind.Utc);
                timestamp = new DateTimeOffset(timeWithoutMilliseconds);
                return true;
            }

            timestamp = default(DateTimeOffset);
            return false;
        }

        public string GetPartitionKey(DateTimeOffset timestamp)
        {
            var e = new LogEvent(timestamp, LogEventLevel.Debug, null, MessageTemplate.Empty, new LogEventProperty[0]);
            var partitionKey = KeyGenerator.GeneratePartitionKey(e);
            return partitionKey;
        }

        public ILogEvent[] SortByTimestamp(IEnumerable<ILogEvent> events)
        {
            var result = new List<ILogEvent>(events);

            bool TryParseCounter(string rowKey, out int value)
            {
                var index = rowKey.LastIndexOf("|", StringComparison.InvariantCulture);
                if (index > 0)
                {
                    var s = rowKey.Substring(index + 1);
                    return int.TryParse(s, out value);
                }
                value = 0;
                return false;
            }

            result.Sort(delegate(ILogEvent e1, ILogEvent e2)
            {
                var comp = e1.Timestamp.CompareTo(e2.Timestamp);
                if (comp == 0)
                {
                    var rowKey1 = e1.Properties["RowKey"] as string;
                    var rowKey2 = e2.Properties["RowKey"] as string;

                    if (TryParseCounter(rowKey1, out var counter1) && TryParseCounter(rowKey2, out var counter2))
                    {
                        comp = counter1.CompareTo(counter2);
                    }
                }
                return comp;
            });

            return result.ToArray();
        }
    }
}
