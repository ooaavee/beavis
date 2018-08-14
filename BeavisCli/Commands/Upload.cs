using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("upload", "A tool for uploading files.")]
    public class Upload : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {            
                return context.Exit();
            });
        }
    }
}
