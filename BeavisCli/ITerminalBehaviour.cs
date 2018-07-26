using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ITerminalBehaviour
    {
        void OnInitialize(HttpContext context, WebCliResponse response);

        bool IsVisibleForHelp(WebCliCommand cmd, WebCliContext context);
    }
}
