using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplication("upload", "Uploads a file.")]
    internal class Upload : WebCliApplication
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() => Exit(context), context);
        }
    }
}
