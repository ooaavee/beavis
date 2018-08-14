using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ICommandExecutionEnvironment
    {
        bool IsAuthorized(CommandContext context);

        bool IsVisibleForHelp(ICommand cmd, HttpContext httpContext);

        bool IsTabCompletionEnabled(ICommand cmd, HttpContext httpContext);
    }
}
