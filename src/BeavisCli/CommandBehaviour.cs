using System;

namespace BeavisCli
{
    public class CommandBehaviour
    {
        public Type ImplementationType { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisibleForHelp { get; set; } = true;

        public bool IsTabCompletionEnabled { get; set; } = true;
    }
}