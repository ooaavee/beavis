using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ICommandExecutionEnvironment
    {
        // TODO: Tämä siirrettävä johonkin toisaalle tästä rajapinnasta!
        bool IsKnownRequestType(BeavisCliRequestTypes type, HttpContext httpContext);

        bool IsAuthorized(CommandContext context);

        bool IsVisibleForHelp(ICommand cmd, HttpContext httpContext);

        bool IsTabCompletionEnabled(ICommand cmd, HttpContext httpContext);
    }
}
