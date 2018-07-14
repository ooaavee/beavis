namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultAuthorizationHandler : IAuthorizationHandler
    {
        public bool IsAuthorized(WebCliApplication application, WebCliContext context)
        {
            // by default every application execution is authorized
            return true;
        }
    }
}
