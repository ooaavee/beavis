using System;

namespace BeavisCli
{
    public class CommandDefinition
    {
        public CommandDefinition(Type implementationType)
        {
            ImplementationType = implementationType;
        }

        public Type ImplementationType { get; }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisibleForHelp { get; set; } = true;

        public bool IsTabCompletionEnabled { get; set; } = true;
    }
}