using System.Threading.Tasks;
using BeavisLogs.Models.DataSources;

namespace BeavisLogs.Drivers
{
    public interface IDriver
    {
        /// <summary>
        /// Executes a query for fetching log data from the data source.
        /// </summary>
        /// <param name="context">defines the actual query</param>
        /// <returns></returns>
        Task ExecuteQueryAsync(QueryContext context);
    }
}
