using BeavisCli.JavaScriptStatements;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Commands
{
    [WebCliCommand("reset", "Reset terminal")]
    internal class Reset : WebCliCommand
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                context.Response.AddJavaScript(new ClearTerminal());
                context.Response.AddJavaScript(new ClearTerminalHistory());
                context.Response.AddJavaScript(new Reload(true));

                return Exit(context);
            }, context);

        }
    }
}
