using System.Collections.Generic;

namespace BeavisCli
{
    public interface IArgument
    {
        List<string> Values { get; }

        bool MultipleValues { get; }

        string Value { get; }
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