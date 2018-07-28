using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;

namespace BeavisCli.Commands
{
    [WebCliCommand("reset", "Reset terminal")]
    public class Reset : WebCliCommand
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
