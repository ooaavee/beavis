using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplication("clear", "Clears the terminal.")]
    internal class Clear : WebCliApplication
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() => Exit(context), context);
        }
    }
}
