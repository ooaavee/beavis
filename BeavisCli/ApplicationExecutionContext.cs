using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class ApplicationExecutionContext
    {
        public ApplicationExecutionContext(ApplicationExecutionRequest request, ApplicationExecutionResponse response, HttpContext httpContext, ICommandLineApplication host, ApplicationInfo info)
        {
            Request = request;
            Response = response;
            HttpContext = httpContext;
            Host = host;
            Info = info;
        }

        /// <summary>
        /// HTTP context
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// Request
        /// </summary>
        public ApplicationExecutionRequest Request { get; }

        /// <summary>
        /// Response
        /// </summary>
        public ApplicationExecutionResponse Response { get; }

        public ICommandLineApplication Host { get; }

        public ApplicationInfo Info { get; }

    }
}
