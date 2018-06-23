using System;

namespace Beavis.Ipc
{
    public class ModuleResponse
    {
        public bool Succeed { get; private set; }

        public Exception Exception { get; private set; }

        public HttpResponseModel Content { get; private set; }

        private ModuleResponse() { }

        public static ModuleResponse CreateSucceed(HttpResponseModel content)
        {
            var response = new ModuleResponse
            {
                Succeed = true,
                Content = content
            };
            return response;
        }

        public static ModuleResponse CreateFailed(Exception exception = null)
        {
            var response = new ModuleResponse
            {
                Succeed = false,
                Exception = exception
            };
            return response;
        }

    }
}