using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public sealed partial class WebCliOptions
    {
        public WebCliOptions()
        {
            BuiltInApplications = GetBuiltInApplications();
        }

        public string Path { get; set; } = "/beaviscli";

        public bool DisplayExceptions { get; set; }

        public bool UseTerminalInitializer { get; set; } = true;

        public Type UnauthorizedHandlerType { get; set; }

        public Type TerminalInitializerType { get; set; }

        public Type FileStorageType { get; set; }

        public Type AuthorizationHandlerType { get; set; }

        public IReadOnlyDictionary<string, BuiltInApplicationBehaviour> BuiltInApplications { get; }

        public class BuiltInApplicationBehaviour
        {
            public bool Enabled { get; set; } = true;
            public bool IsVisibleForHelp { get; set; } = true;
            internal Type Type { get; set; }
        }
    }
}
