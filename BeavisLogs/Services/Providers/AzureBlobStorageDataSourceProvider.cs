using BeavisLogs.Models.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisLogs.Services.Providers
{
    public class AzureBlobStorageDataSourceProvider : IDataSourceProvider
    {
        /// <summary>
        /// Gets all data source available.
        /// </summary>
        /// <returns>all data sources</returns>
        public async Task<IEnumerable<DataSource>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to find a data source by its id or name.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <returns>found data source or null if not found</returns>
        public async Task<DataSource> FindAsync(string idOrName)
        {
            throw  new NotImplementedException();
        }

        /// <summary>
        /// Registers a new data source.
        /// </summary>
        /// <param name="name">data source name</param>
        /// <param name="driverType">driver implementation type</param>
        /// <param name="properties">driver properties</param>
        /// <returns>registered data source</returns>
        /// <exception cref="DataSourceRegistrationException">if registration failed</exception>
        public async Task<DataSource> RegisterAsync(string name, Type driverType, DriverProperties properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters a data source.
        /// </summary>
        /// <param name="item">data source to unregister</param>
        /// <returns></returns>
        /// <exception cref="DataSourceRegistrationException">if unregistration failed</exception>
        public async Task UnregisterAsync(DataSource item)
        {
            throw new NotImplementedException();
        }

    }
}
