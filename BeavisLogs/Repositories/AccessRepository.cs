using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeavisLogs.Drivers.Serilog.AzureTableStorage;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Users;
using BeavisLogs.Repositories;
using BeavisLogs.Repositories.Blob;
using BeavisLogs.Services;

namespace BeavisLogs.Repositories
{
    public class AccessRepository : IAccessRepository
    {
        /*
         * Blobit
         * - DataSource (JSON)
         * - User (JSON)
         * - Admins (text)
         */

        private readonly IAzureBlobStorage _storage;


        public AccessRepository(IAzureBlobStorage storage)
        {
            _storage = storage;
        }



        /// <summary>
        /// Checks if the user has the specified role.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <param name="role">role to check</param>
        /// <returns>true if the user has the specified role</returns>
        public async Task<bool> HasUserRoleAsync(string email, UserRoles role)
        {
            throw  new NotImplementedException();
        }

        /// <summary>
        /// Grants admin privileges to the specified user. If the user doesn't exist, it will be created.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        public async Task GrantAdminPrivilegesAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Revokes admin privileges from the specified user. The user will not be removed.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        public async Task RemoveAdminPrivilegesAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all admin users.
        /// </summary>
        /// <returns>all admin users</returns>
        public async Task<IEnumerable<User>> GetAdminsAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        public async Task AddUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes an existing user.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        public async Task RemoveUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the user data.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <param name="user">user's data</param>
        /// <returns></returns>
        public async Task UpdateUserAsync(string email, UserInfo user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>all users</returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Grants reader privileges to the specified data source for the specified user.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <param name="emails">user's e-mail</param>
        /// <returns></returns>
        public async Task GrantReaderPrivilegesAsync(string idOrName, string emails)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Revokes reader privileges to the specified data source from the specified user.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <param name="emails">user's e-mail</param>
        /// <returns></returns>
        public async Task RevokeReaderPrivilegesAsync(string idOrName, string emails)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all data source available.
        /// </summary>
        /// <returns>all data sources</returns>
        public async Task<IEnumerable<DataSource>> GetDataSourcesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to find a data source by its id or name.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <returns>found data source or null if not found</returns>
        public async Task<DataSource> GetDataSourceAsync(string idOrName)
        {
            DataSource ds = new DataSource();
            ds.Info = new DataSourceInfo() { Id = idOrName };
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
        public async Task<DataSource> AddDataSourceAsync(string name, Type driverType, DriverProperties properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters a data source.
        /// </summary>
        /// <param name="item">data source to unregister</param>
        /// <returns></returns>
        /// <exception cref="DataSourceRegistrationException">if unregistration failed</exception>
        public async Task RemoveDataSourceAsync(DataSource item)
        {
            throw new NotImplementedException();
        }

    }
}

