using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisLogs.Services
{
    public class DataSourceRegistrationException : Exception
    {
        public DataSourceRegistrationException(string message) : base(message)
        {
        }

        public DataSourceRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
