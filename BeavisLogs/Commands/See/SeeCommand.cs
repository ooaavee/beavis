using BeavisCli;
using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisLogs.Commands.See
{
    [Command("see", "A powerful tool for fetching log data from multiple data sources.")]
    public class SeeCommand : ICommand
    {
        private readonly IAccessProvider _access;
        private readonly IDataSourceProvider _dataSources;
        private readonly QueryContextFactory _contextFactory;

        public SeeCommand(IAccessProvider access, IDataSourceProvider dataSources, QueryContextFactory contextFactory)
        {
            _access = access;
            _dataSources = dataSources;
            _contextFactory = contextFactory;
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            // introduce command options



            // data sources for this command execution
            async Task<IEnumerable<DataSource>> GetDataSources()
            {
                DataSource source = await _dataSources.FindAsync("foobar");
                return new[] { source };
            }

            await context.OnExecuteAsync(async () =>
            {
                var handles = new List<(IDriver driver, QueryContext query)>();

                foreach (DataSource source in await GetDataSources())
                {
                    // find driver
                    IDriver driver = DriverLocator.FindDriver(source.DriverType, context.HttpContext);
                    if (driver == null)
                    {
                        return await context.ExitWithError($"Driver '{source.DriverType}' not found.");
                    }
                  
                    QueryContext query = _contextFactory.CreateContext(source);

                    handles.Add((driver, query));
                }

#pragma warning disable CS4014

                Task.Run(async () =>
                {
                    foreach (var (driver, query) in handles)
                    {
                        await driver.ExecuteQueryAsync(query);
                    }
                });

#pragma warning restore CS4014

                return await context.ExitAsync();
            });
        }
    }
}
