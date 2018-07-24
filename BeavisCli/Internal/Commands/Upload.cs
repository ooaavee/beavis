using System.Threading.Tasks;

namespace BeavisCli.Internal.Commands
{
    [WebCliCommand("upload", "Uploads a file.")]
    internal class Upload : WebCliCommand
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {            
                return Exit(context);
            }, context);
        }
    }
}
