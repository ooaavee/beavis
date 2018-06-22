using System;

namespace Beavis.Ipc
{
    public class ModuleResponse
    {
        public bool Succeed { get;  }

        public Exception Exception { get; }

        public HttpResponseModel Content { get;  }

        public ModuleResponse(HttpResponseModel content)
        {
            Succeed = true;
            Content = content;
        }

        public ModuleResponse(Exception exception)
        {
            Succeed = false;
            Exception = exception;
        }
    }
}