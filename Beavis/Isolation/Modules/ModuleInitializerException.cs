using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beavis.Isolation.Modules
{
    public class ModuleInitializerException : Exception
    {
        public ModuleInitializerException(string message) : base(message)
        {
        }

        public ModuleInitializerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
