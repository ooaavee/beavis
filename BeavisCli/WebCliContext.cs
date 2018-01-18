using BeavisCli.Internal;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class WebCliContext
    {
        internal WebCliContext(WebCliRequest request, WebCliResponse response, HttpContext httpContext, WebCliApplicationHost host)
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

        public IOption Option(string template, string description, CommandOptionType optionType)
        {
            return Host.Option(template, description, optionType);
        }

        public IArgument Argument(string name, string description, bool multipleValues = false)
        {
            return Host.Argument(name, description, multipleValues);
        }

        internal WebCliApplicationHost Host { get; }

    }
}
