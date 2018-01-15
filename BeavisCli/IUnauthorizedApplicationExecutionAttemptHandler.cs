using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli
{
    public interface IUnauthorizedApplicationExecutionAttemptHandler
    {
        void HandleUnauthorizedApplicationExecution(ApplicationExecutionContext context);
    }
}
