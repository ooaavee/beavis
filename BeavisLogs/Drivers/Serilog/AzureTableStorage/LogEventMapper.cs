using BeavisLogs.Models.Logs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class LogEventMapper
    {
        public ILogEvent Map(LogEventTableEntity entity, LogEventMappingContext mappingContext)
        {
            // Timestamp
            if (!TryParseTimestamp(entity, mappingContext, out DateTimeOffset timestamp))
            {
                return null;
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
                        return null;
                }
            }
            else
            {
                return null;
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
                  
            ILogEvent e = entity;
           
            e.ReadLogEvent(values);

            return e;
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
    }
}
