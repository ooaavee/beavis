using System;
using System.Collections.Generic;

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
            // TODO: Pitäisiköhän tähän tehdä tarkistus, että template alkaa "-" merkillä?


            Microsoft.Extensions.CommandLineUtils.CommandOptionType type;

            switch (optionType)
            {
                case OptionType.MultipleValue:
                    type = Microsoft.Extensions.CommandLineUtils.CommandOptionType.MultipleValue;
                    break;

                case OptionType.SingleValue:
                    type = Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue;
                    break;

                case OptionType.NoValue:
                    type = Microsoft.Extensions.CommandLineUtils.CommandOptionType.NoValue;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(optionType), optionType, null);
            }

            var opt = _context.Processor.Option(template, description, type);
            return new Option(opt);
        }

        /// <summary>
        /// Creates a new command argument.
        /// </summary>
        public IArgument Argument(string name, string description, bool multipleValues = false)
        {
            var arg = _context.Processor.Argument(name, description, multipleValues);
            return new Argument(arg);
        }
    }

    public interface IOption
    {
        List<string> Values { get; }

        bool HasValue();

        string Value();
    }

    public interface IArgument
    {
        List<string> Values { get; }

        bool MultipleValues { get; }

        string Value { get; }
    }

    public enum OptionType
    {
        MultipleValue,
        SingleValue,
        NoValue
    }

    internal sealed class Option : IOption
    {
        private readonly Microsoft.Extensions.CommandLineUtils.CommandOption _target;

        public Option(Microsoft.Extensions.CommandLineUtils.CommandOption target)
        {
            _target = target;
        }

        public List<string> Values => _target.Values;

        public bool HasValue()
        {
            return _target.HasValue();
        }

        public string Value()
        {
            return _target.Value();
        }
    }

    internal sealed class Argument : IArgument
    {
        private readonly Microsoft.Extensions.CommandLineUtils.CommandArgument _target;

        public Argument(Microsoft.Extensions.CommandLineUtils.CommandArgument target)
        {
            _target = target;
        }

        public List<string> Values => _target.Values;

        public bool MultipleValues => _target.MultipleValues;

        public string Value => _target.Value;
    }
}