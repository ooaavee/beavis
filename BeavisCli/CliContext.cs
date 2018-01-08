using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class CliContext
    {
        public CliContext(CliRequest request, CliResponse response, HttpContext httpContext)
        {
            Request = request;
            Response = response;
            HttpContext = httpContext;
        }

        /// <summary>
        /// HTTP context
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// Request
        /// </summary>
        public CliRequest Request { get; }

        /// <summary>
        /// Response
        /// </summary>
        public CliResponse Response { get; }
    }

}
