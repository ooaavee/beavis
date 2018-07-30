using System.Collections.Generic;

namespace BeavisCli
{
    public interface ICommandOption
    {
        List<string> Values { get; }

        bool HasValue();

        string Value();
    }

    public enum CommandOptionType
    {
        MultipleValue,
        SingleValue,
        NoValue
    }

    internal sealed class CommandOption : ICommandOption
    {
        private readonly Microsoft.Extensions.CommandLineUtils.CommandOption _target;

        public CommandOption(Microsoft.Extensions.CommandLineUtils.CommandOption target)
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
}