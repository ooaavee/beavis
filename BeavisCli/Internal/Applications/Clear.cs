using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplicationDefinition(Name = "clear", Description = "Clears the terminal.")]
    internal class Clear : WebCliApplication
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() => Exit(context), context);
        }
    }
}
