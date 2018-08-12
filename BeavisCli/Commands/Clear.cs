using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("clear", "Clear terminal")]
    public class Clear : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {
                // nothing special here -> all the work will be done on the client-side by the jQuery Terminal component 
                return context.Exit();
            });
        }
    }
}
