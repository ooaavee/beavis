using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;

namespace BeavisCli.Commands
{
    [Command("reset", "Reset terminal")]
    public class Reset : Command
    {
        public override async Task ExecuteAsync(CommandContext context)
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
