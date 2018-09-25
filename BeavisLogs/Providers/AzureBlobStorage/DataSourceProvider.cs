using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeavisLogs.Drivers.Serilog.AzureTableStorage;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Services;
using Microsoft.Extensions.Configuration;

namespace BeavisLogs.Providers.AzureBlobStorage
{
    public class DataSourceProvider : IDataSourceProvider
    {
        private readonly IConfiguration _configuration;

        public DataSourceProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
            DataSource ds = new DataSource();
            ds.DriverType = typeof(Drivers.Serilog.AzureTableStorage.Driver).FullName;
            ds.DriverProperties.Values.Add(Driver.ConnectionString, _configuration["DevConnectionString"]);
            ds.DriverProperties.Values.Add(Driver.TableName, _configuration["DevTableName"]);           
            return ds;
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

