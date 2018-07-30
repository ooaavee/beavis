using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("clear", "Clear terminal")]
    public class Clear : Command
    {
        public override async Task ExecuteAsync(CommandContext context)
        {
            await OnExecuteAsync(() =>
            {
                // nothing special here -> all the work will be done on the client-side by the jQuery Terminal component 
                return Exit(context);
            }, context);
        }
    }
}
