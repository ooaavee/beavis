using System;
using System.IO;
using BeavisCli.Internal;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using CommandArgument = BeavisCli.Microsoft.Extensions.CommandLineUtils.CommandArgument;

namespace BeavisCli
{
    public class WebCliContext
    {
        internal WebCliContext(CommandLineApplication cli,
                               HttpContext httpContext,
                               WebCliRequest request, 
                               WebCliResponse response)
        {
            Cli = cli;
            HttpContext = httpContext;
            Request = request;
            Response = response;
        }

        internal CommandLineApplication Cli { get; }

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
        /// Writer for out message, like Console.Out
        /// </summary>
        public TextWriter OutWriter => Cli.Out;

        /// <summary>
        /// Writer for error messages, like Console.Error
        /// </summary>
        public TextWriter ErrorWriter => Cli.Error;

        public IOption Option(string template, string description, CommandOptionType optionType)
        {
            CommandOption option = Cli.Option(template, description, Map(optionType));
            return new Option(option);
        }

        public IArgument Argument(string name, string description, bool multipleValues = false)
        {
            CommandArgument argument = Cli.Argument(name, description, multipleValues);
            return new Internal.CommandArgument(argument);
        }


        private static Microsoft.Extensions.CommandLineUtils.CommandOptionType Map(CommandOptionType optionType)
        {
            switch (optionType)
            {
                case CommandOptionType.MultipleValue:
                    return Microsoft.Extensions.CommandLineUtils.CommandOptionType.MultipleValue;

                case CommandOptionType.SingleValue:
                    return Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue;

                case CommandOptionType.NoValue:
                    return Microsoft.Extensions.CommandLineUtils.CommandOptionType.NoValue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(optionType), optionType, null);
            }
        }

    }
}
