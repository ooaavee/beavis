using System;

namespace BeavisLogs.Models.Users
{
    public class User
    {
        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Information about the user
        /// </summary>
        public UserInfo Info { get; set; }

        /// <summary>
        /// Checks if the user is administrator
        /// </summary>
        /// <returns>true if the user is administrator, otherwise false</returns>
        public bool IsAdmin()
        {
            throw new NotImplementedException();
        }
    }
}
