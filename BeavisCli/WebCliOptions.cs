using System;

namespace BeavisCli
{
    public sealed class WebCliOptions
    {
        public string Path { get; set; } = "/beavis-cli";

        public bool EnableDefaultApplications { get; set; } = true;

        public bool AllowDefaultApplicationsBrowsing { get; set; } = true;

        public bool EnableFileUpload { get; set; }

        public bool DisplayExceptions { get; set; }

        public bool UseTerminalInitializer { get; set; } = true;

        public Type UnauthorizedHandlerType { get; set; }

        public Type TerminalInitializerType { get; set; }

        public Type FileUploadStorageType { get; set; }

        public Type AuthorizationHandlerType { get; set; }
    }
}
