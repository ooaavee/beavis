using System;

namespace BeavisCli.Internal
{
    internal class WebCliSandboxException : Exception
    {
        public WebCliSandboxException(string message) : base(message)
        {
        }
    }
}