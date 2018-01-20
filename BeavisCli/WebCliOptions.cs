namespace BeavisCli
{
    public class WebCliOptions
    {
        public bool UseDefaultApplications { get; set; }

        public bool AreDefaultApplicationsBrowsable { get; set; }

        public IUnauthorizedHandler UnauthorizedHandler { get; set; }

        public ITerminalInitializer TerminalInitializer { get; set; }
    }
}
