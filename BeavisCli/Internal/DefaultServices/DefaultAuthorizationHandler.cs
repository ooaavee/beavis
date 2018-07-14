namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultAuthorizationHandler : IAuthorizationHandler
    {
        public bool IsAuthorized(WebCliApplication application, WebCliContext context)
        {
            return true;
        }
    }
}
