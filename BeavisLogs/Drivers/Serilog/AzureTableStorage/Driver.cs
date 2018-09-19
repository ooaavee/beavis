using BeavisLogs.Models.Logs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using BeavisLogs.Models.DataSources;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class Driver : IDriver
    {
        private readonly LogEventMapper _mapper;
        private readonly QueryFilterBuilder _filterBuilder;

        public Driver(LogEventMapper mapper, QueryFilterBuilder filterBuilder)
        {
            _mapper = mapper;
            _filterBuilder = filterBuilder;
        }

        /// <summary>
        /// Executes a query for fetching log data from the data source.
        /// </summary>
        /// <param name="context">defines the actual query</param>
        /// <returns></returns>
        public async Task ExecuteQueryAsync(QueryContext context)
        {
            try
            {
                LogEventMappingContext mappingContext = new LogEventMappingContext(context.DriverProperties);

                // Get all needed driver properties.
                string connectionString = context.DriverProperties.Get(Properties.ConnectionString);
                string tableName = context.DriverProperties.Get(Properties.TableName);

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
                QueryFilter filter = _filterBuilder.Build(context);

                // Initialize the continuation token to null to start from the beginning of the table.
                TableContinuationToken continuationToken = null;
               
                do
                {
                    // Retrieve a segment (up to 1,000 entities).
                    TableQuerySegment<LogEventTableEntity> tableQueryResult = await table.ExecuteQuerySegmentedAsync(filter.Query, continuationToken);

                    // Assign the new continuation token to tell the service where to continue on the next
                    // iteration (or null if it has reached the end).
                    continuationToken = tableQueryResult.ContinuationToken;

                    foreach (LogEventTableEntity entity in tableQueryResult.Results)
                    {
                        if (_mapper.TryMap(entity, mappingContext, out ILogEvent e))
                        {
                            if (filter.Success(e))
                            {
                                context.OnFound(e);
                            }
                        }
                    }
                } while (continuationToken != null && context.IsAlive());
            }
            catch (DriverException ex)
            {
                context.OnErrorOccurred(ex);
            }
            catch (Exception ex)
            {
                context.OnErrorOccurred(DriverException.FromException(ex));
            }

            context.OnQueryCompleted();
        }

        public static class Properties
        {
            public const string ConnectionString = "Azure.TableStorage.ConnectionString";
            public const string TableName = "Azure.TableStorage.TableName";
        }
    }
}
