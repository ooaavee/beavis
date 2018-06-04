using System;

namespace Beavis.Isolation.Contracts
{
    public class ResponseEnvolope
    {
        public bool Succeed { get;  }

        public Exception Exception { get; }

        public ModuleResponse Content { get;  }

        public ResponseEnvolope(ModuleResponse content)
        {
            Succeed = true;
            Content = content;
        }

        public ResponseEnvolope(Exception exception)
        {
            Succeed = false;
            Exception = exception;
        }
    }
}