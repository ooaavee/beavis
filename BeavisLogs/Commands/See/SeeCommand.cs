using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisCli;

namespace BeavisLogs.Commands.See
{
    [Command("see", "A powerful tool for fetching log data from multiple data sources.")]
    public class SeeCommand : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(async () =>
            {

                context.WriteText("Whohoo!");

                return await context.ExitAsync();

            });
        }
    }
}
