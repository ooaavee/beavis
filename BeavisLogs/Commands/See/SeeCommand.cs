using BeavisCli;
using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using BeavisLogs.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

                DataSource ds = await _dataSources.FindAsync("foobar");


                IDriver driver = DriverLocator.FindDriver(ds.DriverType, context.HttpContext);


                QueryContext query = new QueryContext();
                query.DriverProperties = ds.DriverProperties;

                //query.Parameters.Levels.Add(LogLevel.Error);
                //query.Parameters.Levels.Add(LogLevel.Critical);


                query.Found += events =>  
                {
                    NotifyFound(context, events);
                };

                query.QueryCompleted += () => 
                {
                    NotifyQueryCompleted(context);
                };

                query.ErrorOccurred += (e) =>
                {
                    NotifyErrorOccurred(context, e);
                };

                await driver.ExecuteQueryAsync(query);


                context.WriteText("Whohoo!");

                return await context.ExitAsync();
            });
        }

        private void NotifyFound(CommandContext context, ILogEvent[] events)
        {
        }

        private void NotifyQueryCompleted(CommandContext context)
        {
        }

        private void NotifyErrorOccurred(CommandContext context, DriverException e)
        {
        }


    }
}
