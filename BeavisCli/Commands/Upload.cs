using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("upload", "A tool for uploading files")]
    public class Upload : Command
    {
        public override async Task ExecuteAsync(CommandContext context)
        {
            await OnExecuteAsync(() =>
            {            
                return Exit(context);
            }, context);
        }
    }
}
