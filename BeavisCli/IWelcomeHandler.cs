using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli
{
    public interface IWelcomeHandler
    {
        void SayWelcome(ApplicationExecutionResponse response);
    }
}
