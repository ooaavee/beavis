namespace BeavisCli
{
    public interface IAuthorizationHandler
    {
        bool IsAuthorized(Command cmd, CommandContext context);
    }
}
