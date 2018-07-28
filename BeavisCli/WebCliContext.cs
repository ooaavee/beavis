using System;
using System.IO;
using BeavisCli.Internal;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class WebCliContext
    {
        internal WebCliContext(CommandLineApplication processor,
                               HttpContext httpContext,
                               WebCliRequest request, 
                               WebCliResponse response)
        {
            Processor = processor;
            HttpContext = httpContext;
            Request = request;
            Response = response;
        }

        internal CommandLineApplication Processor { get; }

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
        public TextWriter OutWriter => Processor.Out;

        /// <summary>
        /// Writer for error messages, like Console.Error
        /// </summary>
        public TextWriter ErrorWriter => Processor.Error;

        public ICommandOption Option(string template, string description, CommandOptionType optionType)
        {
            var option = Processor.Option(template, description, Map(optionType));
            return new CommandOption(option);
        }

        public ICommandArgument Argument(string name, string description, bool multipleValues = false)
        {
            var argument = Processor.Argument(name, description, multipleValues);
            return new CommandArgument(argument);
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
