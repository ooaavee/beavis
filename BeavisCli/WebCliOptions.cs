namespace BeavisCli
{
    public class WebCliOptions
    {
        public bool EnableDefaultApplications { get; set; }

        public bool EnableDefaultApplicationsBrowsing { get; set; }

        public IUnauthorizedHandler UnauthorizedHandler { get; set; }

        public ITerminalInitializer TerminalInitializer { get; set; }

        public IFileUploadStorage FileUploadStorage { get; set; }

        public bool EnableFileUpload { get; set; }

    }
}
