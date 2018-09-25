using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
