using System;

namespace BeavisCli.Internal
{
    internal class ApplicationProviderException : Exception
    {
        public ApplicationProviderException(string message) : base(message)
        {
        }
    }
}