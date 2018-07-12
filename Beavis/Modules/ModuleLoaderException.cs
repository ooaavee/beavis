using System;

namespace Beavis.Modules
{
    public class ModuleLoaderException : Exception
    {
        public ModuleLoaderException(string message) : base(message)
        {
        }

        public ModuleLoaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
