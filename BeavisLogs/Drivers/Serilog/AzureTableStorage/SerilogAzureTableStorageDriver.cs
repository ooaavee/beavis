using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILogEvent = BeavisLogs.Models.Logs.ILogEvent;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class SerilogAzureTableStorageDriver : IDriver
    {
        public const string ConnectionString = "Azure.TableStorage.ConnectionString";
        public const string TableName = "Azure.TableStorage.TableName";

        private readonly LogEventMapper _mapper;
        private readonly QueryBuilder _builder;

        public SerilogAzureTableStorageDriver(LogEventMapper mapper, QueryBuilder builder)
        {
            _mapper = mapper;
            _builder = builder;
        }

        /// <summary>
        /// Executes a query for fetching log data from the data source.
        /// </summary>
        /// <param name="context">defines the actual query</param>
        /// <returns></returns>
        public async Task ExecuteQueryAsync(QueryContext context)
        {
            LogEventMappingContext mappingContext = new LogEventMappingContext(context.DriverProperties);

            // Get all needed driver properties.
            string connectionString = context.DriverProperties.Get(ConnectionString);
            string tableName = context.DriverProperties.Get(TableName);

            // Retrieve the storage account from the connection string.
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new DriverException("Invalid connection string format.");
            }

            // Create the table client.
            CloudTableClient tableClient = account.CreateCloudTableClient();

            // Create the CloudTable that represents the table that contains the log event data.
            CloudTable table = tableClient.GetTableReference(tableName);
            if (!await table.ExistsAsync())
            {
                throw new DriverException($"Table '{tableName}' does not exist.");
            }

            // Build the filter for log events: this is our so called WHERE clause.
            Query filter = _builder.Build(context);

            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            List<ILogEvent> buffer = new List<ILogEvent>();

            bool alive = false;

            do
            {
                // Retrieve a segment (up to 1,000 entities).
                TableQuerySegment<LogEventTableEntity> tableQueryResult = await table.ExecuteQuerySegmentedAsync(filter.TableQuery, continuationToken);

                // Assign the new continuation token to tell the service where to continue on the next
                // iteration (or null if it has reached the end).
                continuationToken = tableQueryResult.ContinuationToken;

                foreach (LogEventTableEntity entity in tableQueryResult.Results)
                {
                    if (_mapper.TryMap(entity, mappingContext, out ILogEvent e))
                    {
                        if (filter.PassFilters(e))
                        {
                            Add(e, buffer, context);
                        }
                    }

                    alive = context.IsAlive();
                    if (!alive)
                    {
                        break;
                    }
                }
            } while (continuationToken != null && alive);

            if (buffer.Any())
            {
                Flush(buffer, context);
            }
        }

        private void Add(ILogEvent e, List<ILogEvent> buffer, QueryContext context)
        {
            if (buffer.Any())
            {
                if (!BufferItem(e, buffer.Last()))
                {
                    Flush(buffer, context);
                }
            }
            buffer.Add(e);
        }

        private void Flush(List<ILogEvent> buffer, QueryContext context)
        {
            ILogEvent[] sorted = _mapper.SortByTimestamp(buffer);
            buffer.Clear();
            context.OnFound(sorted);
        }

        private static bool BufferItem(ILogEvent e, ILogEvent previous)
        {
            DateTimeOffset ts1 = e.Timestamp;
            DateTimeOffset ts2 = previous.Timestamp;

            return ts1.Year == ts2.Year &&
                   ts1.DayOfYear == ts2.DayOfYear &&
                   ts1.Hour == ts2.Hour &&
                   ts1.Minute == ts2.Minute &&
                   ts1.Second == ts2.Second;
        }       
    }
}
