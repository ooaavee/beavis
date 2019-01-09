using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Users;
using BeavisLogs.Services;

namespace BeavisLogs.Repositories
{
    public interface IAccessRepository
    {
        /// <summary>
        /// Checks if the user has the specified role.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <param name="role">role to check</param>
        /// <returns>true if the user has the specified role</returns>
        Task<bool> HasUserRoleAsync(string email, UserRoles role);

        /// <summary>
        /// Grants admin privileges to the specified user. If the user doesn't exist, it will be created.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        Task GrantAdminPrivilegesAsync(string email);

        /// <summary>
        /// Revokes admin privileges from the specified user. The user will not be removed.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        Task RemoveAdminPrivilegesAsync(string email);

        /// <summary>
        /// Gets all admin users.
        /// </summary>
        /// <returns>all admin users</returns>
        Task<IEnumerable<User>> GetAdminsAsync();

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        Task AddUserAsync(string email);

        /// <summary>
        /// Removes an existing user.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <returns></returns>
        Task RemoveUserAsync(string email);

        /// <summary>
        /// Updates the user data.
        /// </summary>
        /// <param name="email">user's e-mail</param>
        /// <param name="user">user's data</param>
        /// <returns></returns>
        Task UpdateUserAsync(string email, UserInfo user);

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>all users</returns>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// Grants reader privileges to the specified data source for the specified user.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <param name="emails">user's e-mail</param>
        /// <returns></returns>
        Task GrantReaderPrivilegesAsync(string idOrName, string emails);

        /// <summary>
        /// Revokes reader privileges to the specified data source from the specified user.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <param name="emails">user's e-mail</param>
        /// <returns></returns>
        Task RevokeReaderPrivilegesAsync(string idOrName, string emails);

        /// <summary>
        /// Gets all data source available.
        /// </summary>
        /// <returns>all data sources</returns>
        Task<IEnumerable<DataSource>> GetDataSourcesAsync();

        /// <summary>
        /// Tries to find a data source by its id or name.
        /// </summary>
        /// <param name="idOrName">data source id or name</param>
        /// <returns>found data source or null if not found</returns>
        Task<DataSource> GetDataSourceAsync(string idOrName);

        /// <summary>
        /// Registers a new data source.
        /// </summary>
        /// <param name="name">data source name</param>
        /// <param name="driverType">driver implementation type</param>
        /// <param name="properties">driver properties</param>
        /// <returns>registered data source</returns>
        /// <exception cref="DataSourceRegistrationException">if registration failed</exception>
        Task<DataSource> AddDataSourceAsync(string name, Type driverType, DriverProperties properties);

        /// <summary>
        /// Unregisters a data source.
        /// </summary>
        /// <param name="item">data source to unregister</param>
        /// <returns></returns>
        /// <exception cref="DataSourceRegistrationException">if unregistration failed</exception>
        Task RemoveDataSourceAsync(DataSource item);
    }
}
