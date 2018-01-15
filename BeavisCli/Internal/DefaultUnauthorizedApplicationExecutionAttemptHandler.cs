using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli.Internal
{
    /// <summary>
    /// Handler for unauthorized application execution attempts.
    /// </summary>
    internal class DefaultUnauthorizedApplicationExecutionAttemptHandler : IUnauthorizedApplicationExecutionAttemptHandler
    {
        public void HandleUnauthorizedApplicationExecution(ApplicationExecutionContext context)
        {
            context.Response.WriteError("Unauthorized");
        }
    }
}
