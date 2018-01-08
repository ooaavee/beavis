using System;

namespace BeavisCli.Internal
{
    internal class ApplicationSpaceException : Exception
    {
        public ApplicationSpaceException(string message) : base(message)
        {
        }
    }
}