using BeavisCli.Internal.Applications;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public sealed partial class WebCliOptions
    {
        public WebCliOptions()
        {
            DefaultApplications = InitDefaultTypes();
        }

        public string Path { get; set; } = "/beavis-cli";

        ////public bool EnableDefaultApplications { get; set; } = true;

        ////public bool AllowDefaultApplicationsBrowsing { get; set; } = true;

        ////public bool EnableFileUpload { get; set; }

        public bool DisplayExceptions { get; set; }

        public bool UseTerminalInitializer { get; set; } = true;

        public Type UnauthorizedHandlerType { get; set; }

        public Type TerminalInitializerType { get; set; }

        public Type FileStorageType { get; set; }

        public Type AuthorizationHandlerType { get; set; }

        public IReadOnlyDictionary<string, DefaultApplicationBehaviour> DefaultApplications { get; }

        public class DefaultApplicationBehaviour
        {
            internal Type Type { get; set; }
            public bool Enabled { get; set; }
            public bool IsVisibleForHelp { get; set; }
        }

    }
}
