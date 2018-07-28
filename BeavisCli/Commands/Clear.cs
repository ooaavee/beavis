using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [WebCliCommand("clear", "Clear terminal")]
    public class Clear : WebCliCommand
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                // nothing special here -> all the work will be done on the client-side by the jQuery Terminal component 
                return Exit(context);
            }, context);
        }
    }
}
