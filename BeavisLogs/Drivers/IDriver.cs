using System.Threading.Tasks;

namespace BeavisLogs.Drivers
{
    public interface IDriver
    {
        /// <summary>
        /// Executes a query for fetching log data from the data source.
        /// </summary>
        /// <param name="request">defines the actual query</param>
        /// <returns>query result</returns>
        Task<QueryResult> ExecuteQueryAsync(QueryRequest request);
    }
}
