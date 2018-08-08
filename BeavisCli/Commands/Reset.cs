using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;

namespace BeavisCli.Commands
{
    [Command("reset", "Reset terminal")]
    public class Reset : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {
                context.Response.AddJavaScript(new ClearTerminal());
                context.Response.AddJavaScript(new ClearTerminalHistory());
                context.Response.AddJavaScript(new Reload(true));

                return context.Exit();
            });

        }
    }
}
