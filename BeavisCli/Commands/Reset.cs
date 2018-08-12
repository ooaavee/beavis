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
                context.WriteJs(new ClearTerminal());
                context.WriteJs(new ClearTerminalHistory());
                context.WriteJs(new Reload(true));

                return context.Exit();
            });

        }
    }
}
