using System;

namespace BeavisLogs.Models.Users
{
    [Flags]
    public enum UserRoles
    {
        Administrator = 1,
        Reader = 2
    }
}
