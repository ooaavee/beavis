using System;

namespace BeavisCli
{
    public class CommandBuilder
    {
        private readonly CommandContext _context;

        public CommandBuilder(CommandContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new command options.
        /// </summary>
        public IOption Option(string template, string description, OptionType optionType = OptionType.SingleValue)
        {
            Microsoft.Extensions.CommandLineUtils.CommandOptionType o;

            switch (optionType)
            {
                case OptionType.MultipleValue:
                    o = Microsoft.Extensions.CommandLineUtils.CommandOptionType.MultipleValue;
                    break;
                case OptionType.SingleValue:
                    o = Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue;
                    break;
                case OptionType.NoValue:
                    o = Microsoft.Extensions.CommandLineUtils.CommandOptionType.NoValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionType), optionType, null);
            }

            return new Option(_context.Processor.Option(template, description, o));
        }

        /// <summary>
        /// Creates a new command argument.
        /// </summary>
        public IArgument Argument(string name, string description, bool multipleValues = false)
        {
            return new Argument(_context.Processor.Argument(name, description, multipleValues));
        }

    }
}