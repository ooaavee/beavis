using System;

namespace BeavisCli
{
    public class CommandDefinition
    {
        public bool IsEnabled { get; set; } = true;

        public bool IsVisibleForHelp { get; set; } = true;

        public bool IsTabCompletionEnabled { get; set; } = true;

        internal Type ImplementationType { get; set; }
    }
}