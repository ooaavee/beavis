using System;

namespace BeavisLogs.Drivers
{
    public sealed class DriverException : Exception
    {
        private const string GenericErrorMessage = "An error occurred.";

        public bool Plain { get; set; } = true;

        public DriverException(string message) : base(message)
        {
        }

        public DriverException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static DriverException FromException(Exception exception)
        {
            return new DriverException(GenericErrorMessage, exception) {Plain = false};
        }
    }
}
