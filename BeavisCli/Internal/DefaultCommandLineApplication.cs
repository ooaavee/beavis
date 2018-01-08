using System;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;

namespace BeavisCli.Internal
{
    internal class DefaultCommandLineApplication : ICommandLineApplication
    {
        private readonly CommandLineApplication _target;
        private readonly AbstractApplication _service;

        public DefaultCommandLineApplication(CommandLineApplication target, AbstractApplication service)
        {
            _target = target;
            _service = service;
        }

        public CommandLineApplication Target => _target;

        public ICommandOption Option(string template, string description, CommandOptionType optionType)
        {
            Microsoft.Extensions.CommandLineUtils.CommandOptionType v;
            switch (optionType)
            {
                case CommandOptionType.MultipleValue:
                    v = Microsoft.Extensions.CommandLineUtils.CommandOptionType.MultipleValue;
                    break;
                case CommandOptionType.SingleValue:
                    v = Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue;
                    break;
                case CommandOptionType.NoValue:
                    v = Microsoft.Extensions.CommandLineUtils.CommandOptionType.NoValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionType), optionType, null);
            }

            var obj = _target.Option(template, description, v);

            return new DefaultCommandOption(obj);
        }

        public ICommandArgument Argument(string name, string description, bool multipleValues = false)
        {
            var obj = _target.Argument(name, description, multipleValues);
            return new DefaultCommandArgument(obj);
        }
    }
}