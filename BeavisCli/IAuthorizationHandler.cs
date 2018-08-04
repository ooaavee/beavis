using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IAuthorizationHandler
    {
        bool IsKnownRequestType(BeavisCliRequestTypes type, HttpContext httpContext);

        bool IsAuthorized(Command cmd, CommandContext context);

        bool IsVisibleForHelp(Command cmd, CommandContext context);

        bool IsUploadEnabled(HttpContext httpContext);

        bool IsTabCompletionEnabled(Command cmd, HttpContext httpContext);
    }
}
