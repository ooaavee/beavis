using System.Collections.Generic;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;

namespace BeavisCli.Internal
{
    internal class DefaultCommandArgument : ICommandArgument
    {
        private readonly CommandArgument _target;

        public DefaultCommandArgument(CommandArgument target)
        {
            _target = target;
        }

        public List<string> Values => _target.Values;

        public bool MultipleValues => _target.MultipleValues;

        public string Value => _target.Value;
    }
}