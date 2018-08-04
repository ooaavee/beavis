using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ICommandExecutionEnvironment
    {
        bool IsKnownRequestType(BeavisCliRequestTypes type, HttpContext httpContext);

        bool IsAuthorized(Command cmd, CommandContext context);

        bool IsVisibleForHelp(Command cmd, CommandContext context);

        bool IsTabCompletionEnabled(Command cmd, HttpContext httpContext);
    }
}
