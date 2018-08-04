using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace BeavisCli
{
    public class CommandContext
    {
        private readonly CommandLineApplication _processor;
        private TextWriter _outWriter;
        private TextWriter _errorWriter;
        private HttpContext _httpContext;
        private Request _request;
        private Response _response;

        public CommandContext()
        {
        }

        internal CommandContext(CommandLineApplication processor, HttpContext httpContext, Request request, Response response)
        {
            _processor = processor;
            _outWriter = processor.Out;
            _errorWriter = processor.Error;
            _httpContext = httpContext;
            _request = request;
            _response = response;
        }

        internal CommandLineApplication Processor => _processor;

        /// <summary>
        /// HTTP context
        /// </summary>
        public virtual HttpContext HttpContext
        {
            get => _httpContext;
            set => _httpContext = value;
        }

        /// <summary>
        /// Request
        /// </summary>
        public virtual Request Request
        {
            get => _request;
            set => _request = value;
        }

        /// <summary>
        /// Response
        /// </summary>
        public virtual Response Response
        {
            get => _response;
            set => _response = value;
        }

        /// <summary>
        /// Writer for out message, like Console.Out
        /// </summary>
        public virtual TextWriter OutWriter
        {
            get => _outWriter;
            set => _outWriter = value;
        }

        /// <summary>
        /// Writer for error messages, like Console.Error
        /// </summary>
        public virtual TextWriter ErrorWriter
        {
            get => _errorWriter;
            set => _errorWriter = value;
        }

        public virtual ICommandOption Option(string template, string description, CommandOptionType optionType)
        {
            return new CommandOption(_processor.Option(template, description, MapCommandOptionType(optionType)));
        }

        public virtual ICommandArgument Argument(string name, string description, bool multipleValues = false)
        {
            return new CommandArgument(_processor.Argument(name, description, multipleValues));
        }

        private static Microsoft.Extensions.CommandLineUtils.CommandOptionType MapCommandOptionType(CommandOptionType optionType)
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
