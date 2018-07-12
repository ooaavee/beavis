using System;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;

namespace BeavisCli.Internal
{
    internal class WebCliApplicationHost
    {
        public WebCliApplicationHost(CommandLineApplication cli)
        {
            Cli = cli;
        }

        public CommandLineApplication Cli { get; }

        public IOption Option(string template, string description, CommandOptionType optionType)
        {
            CommandOption option = Cli.Option(template, description, Map(optionType));

            return new Option(option);
        }

        public IArgument Argument(string name, string description, bool multipleValues = false)
        {
            var obj = Cli.Argument(name, description, multipleValues);
            return new CommandArgument(obj);
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