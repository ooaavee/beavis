using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class WebCliContext
    {
        internal WebCliContext(WebCliRequest request, WebCliResponse response, HttpContext httpContext, ICommandLineApplication host)
        {
            Request = request;
            Response = response;
            HttpContext = httpContext;
            Host = host;
        }

        /// <summary>
        /// HTTP context
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// Request
        /// </summary>
        public WebCliRequest Request { get; }

        /// <summary>
        /// Response
        /// </summary>
        public WebCliResponse Response { get; }

        /// <summary>
        /// Underlying cli application
        /// </summary>
        public ICommandLineApplication Host { get; }
    }
}
