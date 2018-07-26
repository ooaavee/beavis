namespace BeavisCli
{
    public interface IAuthorizationHandler
    {
        bool IsAuthorized(WebCliCommand cmd, WebCliContext context);
    }
}
