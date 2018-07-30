using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ITerminalBehaviour
    {
        void OnInitialize(HttpContext context, Response response);

        bool IsVisibleForHelp(Command cmd, CommandContext context);
    }
}
