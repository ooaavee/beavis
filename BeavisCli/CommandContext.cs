﻿using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace BeavisCli
{
    public class CommandContext
    {
        internal CommandContext(CommandLineApplication processor,
                                HttpContext httpContext,
                                Request request, 
                                Response response)
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
        public Request Request { get; }

        /// <summary>
        /// Response
        /// </summary>
        public Response Response { get; }

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
            return new CommandOption(Processor.Option(template, description, Map(optionType)));
        }

        public ICommandArgument Argument(string name, string description, bool multipleValues = false)
        {
            return new CommandArgument(Processor.Argument(name, description, multipleValues));
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
