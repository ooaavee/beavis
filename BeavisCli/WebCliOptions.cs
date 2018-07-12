using System;

namespace BeavisCli
{
    public class WebCliOptions
    {
        public string Path { get; set; } = "/beavis-cli";

        public bool EnableDefaultApplications { get; set; } = true;

        public bool EnableDefaultApplicationsBrowsing { get; set; } = true;

        public bool EnableFileUpload { get; set; } = true;

        public Type UnauthorizedHandlerType { get; set; } = typeof(DefaultUnauthorizedHandler);

        public Type TerminalInitializerType { get; set; } = typeof(DefaultWebCliInitializer);

        public Type FileUploadStorageType { get; set; }
    }
}
