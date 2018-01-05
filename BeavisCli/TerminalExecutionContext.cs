using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class TerminalExecutionContext
    {
        public TerminalExecutionContext(TerminalRequest request, TerminalResponse response, HttpContext httpContext)
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
        public TerminalRequest Request { get; }

        /// <summary>
        /// Response
        /// </summary>
        public TerminalResponse Response { get; }
    }

}
