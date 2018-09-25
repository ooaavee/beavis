using System.Collections.Generic;

namespace BeavisCli
{
    public interface IOption
    {
        List<string> Values { get; }

        bool HasValue();

        string Value();
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
}