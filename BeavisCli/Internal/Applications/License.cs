using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplication("license", "Displays license information.")]
    internal class License : WebCliApplication
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() => Exit(context), context);
        }
    }
}
