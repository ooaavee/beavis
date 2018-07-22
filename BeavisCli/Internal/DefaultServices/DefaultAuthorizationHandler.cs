namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultAuthorizationHandler : IAuthorizationHandler
    {
        public bool IsAuthorized(WebCliApplication application, WebCliContext context)
        {
            // By default every application execution is authorized. If you want some custom 
            // authorization logic, you should
            // - write your own IAuthorizationHandler implementation and register it during the startup or/and
            // - override WebCliApplication.IsAuthorized method
            return true;
        }
    }
}
