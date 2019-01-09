using System;

namespace BeavisLogs.Models.Users
{
    [Flags]
    public enum UserRoles
    {
        Admin = 1,
        Reader = 2
    }
}
