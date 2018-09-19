using System.Threading.Tasks;
using BeavisLogs.Models.Users;

namespace BeavisLogs.Services
{
    public interface IAccessProvider
    {
        /// <summary>
        /// Checks if the user has the specified role.
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="role">role to check</param>
        /// <returns>true if the user has the specified role</returns>
        Task<bool> HasRoleAsync(User user, UserRoles role);
    }
}
