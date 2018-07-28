using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [WebCliCommand("upload", "A tool for uploading files")]
    public class Upload : WebCliCommand
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
