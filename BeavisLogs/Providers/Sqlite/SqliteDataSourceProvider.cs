using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeavisLogs.Drivers.Serilog.AzureTableStorage;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Services;

namespace BeavisLogs.Providers.Sqlite
{
    public class SqliteDataSourceProvider : IDataSourceProvider
    {
        private readonly ConfigurationAccessor _configuration;

        public SqliteDataSourceProvider(ConfigurationAccessor configuration)
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
            ds.Info = new DataSourceInfo() {Id = idOrName};
            ds.DriverType = typeof(Drivers.Serilog.AzureTableStorage.SerilogAzureTableStorageDriver).FullName;
            ds.DriverProperties.Values.Add(SerilogAzureTableStorageDriver.ConnectionString, _configuration["DevConnectionString"]);

            if (idOrName == "1")
            {
                ds.DriverProperties.Values.Add(SerilogAzureTableStorageDriver.TableName, _configuration["DevTableName1"]);
            }
            else if (idOrName == "2")
            {
                ds.DriverProperties.Values.Add(SerilogAzureTableStorageDriver.TableName, _configuration["DevTableName2"]);
            }
            else if (idOrName == "3")
            {
                ds.DriverProperties.Values.Add(SerilogAzureTableStorageDriver.TableName, _configuration["DevTableName3"]);
            }

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

