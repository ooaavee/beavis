using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli
{
    public class BeavisCliOptions
    {
        public bool UseDefaultApplications { get; set; }

        public IUnauthorizedApplicationExecutionAttemptHandler UnauthorizedApplicationExecutionAttemptHandler { get; set; }

        public IWelcomeHandler WelcomeHandler { get; set; }

        //        public IWelcomeHandler IWelcomeHandler { get; set; }

        // - IWelcomeHandler
        // - IUnauthorizedApplicationExecutionAttemptHandler

    }
}
