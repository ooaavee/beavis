using System;
using BeavisCli;
using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BeavisLogs.Drivers.Renderers;
using BeavisLogs.Jobs;
using BeavisLogs.Repositories;
using Microsoft.Extensions.Logging;

namespace BeavisLogs.Commands.See
{
    [Command("see", "A powerful tool for fetching log data from multiple data sources.")]
    public class SeeCommand : ICommand
    {
        private readonly RepositoryFactory _factory;
        private readonly LogEventTempStorage _storage;

        public SeeCommand(RepositoryFactory factory, LogEventTempStorage storage)
        {
            _factory = factory;
            _storage = storage;
        }

        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            // introduce command options

            // -range
            // A timespan, or time range, for a query specified as a ISO 8601 date time or duration string (mandatory).
            IOption optFrom = builder.Option("-from", "Search log events occurred on or after; ISO 8601 date time or duration string relative to the current time.", OptionType.SingleValue);
            IOption optUntil = builder.Option("-until", "Search log events occurred on or before; ISO 8601 date time or duration string relative to the current time.", OptionType.SingleValue);

            // -source
            // A data source name or id for a query (mandatory).
            IOption optSource = builder.Option("-source", "Search log events from the data source; data source id or name.", OptionType.SingleValue);

            // -level
            // A minimum log level for a query, supported values are X.
            IOption optLevels = builder.Option("-level", "Levels", OptionType.SingleValue);

            // -text
            // A text...

            // -regex
            // A regular expression...

            var repo = _factory.Create();

            // data sources for this command execution
            async Task<IEnumerable<DataSource>> GetDataSources()
            {
                return new[]
                {
                    await repo.GetDataSourceAsync("1"),
                    await repo.GetDataSourceAsync("2"),
                    await repo.GetDataSourceAsync("3")
                };
            }

            await context.OnExecuteAsync(async () =>
            {

                //var l = optLevels.Values;

                //var sources = new List<DataSource>();

                //string idOrName;

                var handles = new List<(IDriver driver, QueryContext query)>();

                DataSourceInfo[] sources = (await GetDataSources()).Select(x => x.Info).ToArray();

                string slotKey = KeyUtil.GenerateKey();

                LogEventSlotProperties properties = new LogEventSlotProperties();
//                properties.Renderer = new LogEventTerminalRenderer();
                properties.Renderer = new FileRenderer(sources, FileRenderBehaviour.SingleFiles);


                //properties.Limit = 103;
                properties.Sources = sources;
                properties.Key = slotKey;

                LogEventSlot slot = new LogEventSlot(properties);
                _storage.Set(slot);


                foreach (DataSource source in await GetDataSources())
                {
                    // find driver
                    IDriver driver = DriverLocator.FindDriver(source.DriverType, context.HttpContext);
                    if (driver == null)
                    {
                        return await context.ExitWithError($"Driver '{source.DriverType}' not found.");
                    }


                    QueryContext query = new QueryContext(slot, source.Info, source.DriverProperties);

                    //query.Parameters.Levels.Add(LogLevel.Error);
                    //query.Parameters.Levels.Add(LogLevel.Critical);

                    // query.Parameters.MessageAndExceptionPattern = "(?i)hinweggeschwunden die die folgt läng";

                    //query.Parameters.MessageAndExceptionText = "hinweggeschwunden die die folgt läng";

                    //query.Parameters.PropertyPattern.Add("Exception", "DataException");
                    //query.Parameters.PropertyText.Add("Exception", "DataException");


                    query.Parameters.Levels.Add(LogLevel.Error);

                    query.Parameters.From = DateTimeOffset.Now.AddDays(-100);
                    query.Parameters.Until = DateTimeOffset.Now;

                    handles.Add((driver, query));
                }

#pragma warning disable CS4014
                // run queries in background tasks
                Task.Run(async () =>
                {
                    foreach (var (driver, query) in handles)
                    {
                        try
                        {
                            query.OnQueryStarted();

                            await driver.ExecuteQueryAsync(query);
                        }
                        catch (Exception ex)
                        {
                            query.OnErrorOccurred(ex);
                        }
                        finally
                        {
                            query.OnQueryCompleted();
                        }
                    }
                });
#pragma warning restore CS4014

                // initialize a client-side job that polls log events from the server
                IJob job = new PollLogEventsJob(slotKey);
                context.AddJob(job);

                return await context.ExitAsync();
            });
        }
    }
}
