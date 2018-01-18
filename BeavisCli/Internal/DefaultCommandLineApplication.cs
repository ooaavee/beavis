using System;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;

namespace BeavisCli.Internal
{
    internal class DefaultCommandLineApplication : ICommandLineApplication
    {
        private readonly CommandLineApplication _cli;

        public DefaultCommandLineApplication(CommandLineApplication cli)
        {
            _cli = cli;
        }

        public CommandLineApplication Cli => _cli;

        public IOption Option(string template, string description, CommandOptionType optionType)
        {
            CommandOption option = _cli.Option(template, description, Map(optionType));

            return new DefaultCommandOption(option);
        }

        public IArgument Argument(string name, string description, bool multipleValues = false)
        {
            var obj = _cli.Argument(name, description, multipleValues);
            return new DefaultCommandArgument(obj);
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