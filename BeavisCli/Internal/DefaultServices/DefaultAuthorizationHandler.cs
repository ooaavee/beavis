namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultAuthorizationHandler : IAuthorizationHandler
    {
        public bool IsAuthorized(WebCliCommand command, WebCliContext context)
        {
            // By default every command execution is authorized. If you want some custom 
            // authorization logic, you should
            // - write your own IAuthorizationHandler implementation and register it during the startup or/and
            // - override WebCliCommand.IsAuthorized method
            return true;
        }
    }
}
