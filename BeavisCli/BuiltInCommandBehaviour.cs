using System;

namespace BeavisCli
{
    public class BuiltInCommandBehaviour
    {
        public bool Enabled { get; set; } = true;
        public bool IsVisibleForHelp { get; set; } = true;
        internal Type Type { get; set; }
    }
}