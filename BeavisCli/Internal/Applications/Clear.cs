using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplication("clear", "Clears the terminal.")]
    internal class Clear : WebCliApplication
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
