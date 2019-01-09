namespace BeavisLogs.Drivers
{
    /// <summary>
    /// An exception that is thrown when a query parameter is not supported by the driver.
    /// </summary>
    public class QueryParameterNotSupportedException : DriverException
    {
        public QueryParameterNotSupportedException(string message) : base(message)
        {
        }
    }
}
