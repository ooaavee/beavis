namespace BeavisCli
{
    public class WebCliOptions
    {
        public bool UseDefaultApplications { get; set; }

        public IUnauthorizedHandler UnauthorizedHandler { get; set; }

        public IGreeter Greeter { get; set; }
    }
}
