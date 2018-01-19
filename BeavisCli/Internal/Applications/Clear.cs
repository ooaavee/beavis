using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    internal class Clear : WebCliApplication
    {
        public Clear() : base("clear", "Clears the terminal.") { }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() => Exit(context), context);
        }
    }
}
