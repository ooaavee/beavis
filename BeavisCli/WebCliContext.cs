using System;
using BeavisCli.Internal;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class WebCliContext
    {
        private readonly ICommandLineApplication _host;

        internal WebCliContext(WebCliRequest request, WebCliResponse response, HttpContext httpContext, ICommandLineApplication host)
        {
            Request = request;
            Response = response;
            HttpContext = httpContext;
            _host = host;
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


        internal CommandLineApplication GetHost()
        {
            if (!(_host is DefaultCommandLineApplication obj))
            {
                throw new InvalidOperationException($"Cannot find the {nameof(DefaultCommandLineApplication)} object, operation terminated!");
            }
            return obj.Cli;
        }

        public IOption Option(string template, string description, CommandOptionType optionType)
        {
            return _host.Option(template, description, optionType);
        }

        public IArgument Argument(string name, string description, bool multipleValues = false)
        {
            return _host.Argument(name, description, multipleValues);
        }

    }
}
