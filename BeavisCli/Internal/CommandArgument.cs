using System.Collections.Generic;

namespace BeavisCli.Internal
{
    internal class CommandArgument : IArgument
    {
        private readonly Microsoft.Extensions.CommandLineUtils.CommandArgument _target;

        public CommandArgument(Microsoft.Extensions.CommandLineUtils.CommandArgument target)
        {
            _target = target;
        }

        public List<string> Values => _target.Values;

        public bool MultipleValues => _target.MultipleValues;

        public string Value => _target.Value;
    }
}