using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace BeavisCli
{
    public class CommandResult
    {
        public static readonly CommandResult Default = new CommandResult();

        private const int ExitStatusCode = 2;

        public virtual int StatusCode { get; } = ExitStatusCode;
    }
}
