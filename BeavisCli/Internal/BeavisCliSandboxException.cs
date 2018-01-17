using System;

namespace BeavisCli.Internal
{
    internal class BeavisCliSandboxException : Exception
    {
        public BeavisCliSandboxException(string message) : base(message)
        {
        }
    }
}