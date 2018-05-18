using System;
using BeavisCli.Internal;

namespace BeavisCli
{
    public class WebCliOptions
    {
        public bool EnableDefaultApplications { get; set; } = true;

        public bool EnableDefaultApplicationsBrowsing { get; set; } = true;

        public bool EnableFileUpload { get; set; } = true;

        public Type UnauthorizedHandlerType { get; set; } = typeof(DefaultUnauthorizedHandler);

        public Type TerminalInitializerType { get; set; } = typeof(DefaultTerminalInitializer);

        public Type FileUploadStorageType { get; set; } = typeof(DefaultFileUploadStorage);


    }
}
