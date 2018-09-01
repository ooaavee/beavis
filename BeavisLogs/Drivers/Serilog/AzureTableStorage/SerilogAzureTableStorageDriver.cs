using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public class SerilogAzureTableStorageDriver : IDriver
    {
        /// <summary>
        /// Executes a query for fetching log data from the data source.
        /// </summary>
        /// <param name="request">defines the actual query</param>
        /// <returns>query result</returns>
        public async Task<QueryResult> ExecuteQueryAsync(QueryRequest request)
        {
           

            return null;
        }
    }
}
