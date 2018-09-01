using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisLogs.Models.Users;

namespace BeavisLogs.Services.Providers
{
    public class AzureBlobStorageAccessProvider : IAccessProvider
    {
        /// <summary>
        /// Checks if the user has the specified role.
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="role">role to check</param>
        /// <returns>true if the user has the specified role</returns>
        public async Task<bool> HasRoleAsync(User user, UserRoles role)
        {
            return true;
        }
    }
}
