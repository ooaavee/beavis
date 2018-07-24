namespace BeavisCli
{
    public interface IAuthorizationHandler
    {
        bool IsAuthorized(WebCliCommand command, WebCliContext context);
    }
}
