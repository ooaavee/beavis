using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    internal class Upload : WebCliApplication
    {
        public Upload() : base("upload", "Uploads a file.") { }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() => Exit(context), context);
        }
    }
}
