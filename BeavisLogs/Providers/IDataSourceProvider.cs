using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Services;

namespace BeavisLogs.Providers
{
    public interface IDataSourceProvider
    {
        /// <summary>
        /// Gets all data source available.
        /// </summary>
        /// <returns>all data sources</returns>
        Task<IEnumerable<DataSource>> GetAllAsync();

        /// <summary>
        /// Tries to find a data source by its id or name.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <returns>found data source or null if not found</returns>
        Task<DataSource> FindAsync(string idOrName);

        /// <summary>
        /// Registers a new data source.
        /// </summary>
        /// <param name="name">data source name</param>
        /// <param name="driverType">driver implementation type</param>
        /// <param name="properties">driver properties</param>
        /// <returns>registered data source</returns>
        /// <exception cref="DataSourceRegistrationException">if registration failed</exception>
        Task<DataSource> RegisterAsync(string name, Type driverType, DriverProperties properties);

        /// <summary>
        /// Unregisters a data source.
        /// </summary>
        /// <param name="item">data source to unregister</param>
        /// <returns></returns>
        /// <exception cref="DataSourceRegistrationException">if unregistration failed</exception>
        Task UnregisterAsync(DataSource item);
    }
}
