using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BeavisCli
{
    public interface ITerminalBehaviour
    {
        bool IsRequestHandlerAccessible(HttpContext httpContext, BeavisCliRequestTypes type);

        IEnumerable<ResponseMessage> EnumInitMessages(HttpContext httpContext);

        IEnumerable<IJavaScriptStatement> EnumInitStatements(HttpContext httpContext);

        bool IsVisibleForHelp(Command cmd, CommandContext context);

        bool IsUploadEnabled(HttpContext httpContext);
    }
}
