using System;

namespace BeavisLogs.Drivers
{
    public class DriverException : Exception
    {
        public DriverException(string message) : base(message)
        {
        }

        public DriverException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
