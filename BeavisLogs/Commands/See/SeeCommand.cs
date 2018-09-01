using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisCli;
using BeavisLogs.Services;

namespace BeavisLogs.Commands.See
{
    [Command("see", "A powerful tool for fetching log data from multiple data sources.")]
    public class SeeCommand : ICommand
    {
        private readonly IAccessProvider _access;
        private readonly IDataSourceProvider _dataSources;

        public SeeCommand(IAccessProvider access, IDataSourceProvider dataSources)
        {
            _access = access;
            _dataSources = dataSources;
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            // introduce command options


            await context.OnExecuteAsync(async () =>
            {

                context.WriteText("Whohoo!");

                return await context.ExitAsync();

            });
        }
    }
}
