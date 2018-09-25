using System;
using BeavisCli;
using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BeavisLogs.Providers;
using Microsoft.Extensions.Logging;

namespace BeavisLogs.Commands.See
{
    [Command("see", "A powerful tool for fetching log data from multiple data sources.")]
    public class SeeCommand : ICommand
    {
        private readonly IAccessProvider _accessProvider;
        private readonly IDataSourceProvider _dataSourceProvider;
        private readonly LogEventTempStorage _temp;

        public SeeCommand(IAccessProvider accessProvider, IDataSourceProvider dataSourceProvider, LogEventTempStorage temp)
        {
            _accessProvider = accessProvider;
            _dataSourceProvider = dataSourceProvider;
            _temp = temp;
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


            // data sources for this command execution
            async Task<IEnumerable<DataSource>> GetDataSources()
            {
                DataSource source = await _dataSourceProvider.FindAsync("foobar");
                return new[] { source };
            }

            await context.OnExecuteAsync(async () =>
            {
               
                //var l = optLevels.Values;

                //var sources = new List<DataSource>();

                //string idOrName;

                var handles = new List<(IDriver driver, QueryContext query)>();

                foreach (DataSource source in await GetDataSources())
                {
                    // find driver
                    IDriver driver = DriverLocator.FindDriver(source.DriverType, context.HttpContext);
                    if (driver == null)
                    {
                        return await context.ExitWithError($"Driver '{source.DriverType}' not found.");
                    }
                  
                    QueryContext query = CreateQueryContext(source);

                    //query.Parameters.Levels.Add(LogLevel.Error);
                    //query.Parameters.Levels.Add(LogLevel.Critical);

                   // query.Parameters.MessageAndExceptionPattern = "(?i)hinweggeschwunden die die folgt läng";

                    //query.Parameters.MessageAndExceptionText = "hinweggeschwunden die die folgt läng";

                    //query.Parameters.PropertyPattern.Add("Exception", "DataException");
                    //query.Parameters.PropertyText.Add("Exception", "DataException");


                    query.Parameters.From = DateTimeOffset.Now.AddDays(-100);
                    query.Parameters.Until = DateTimeOffset.Now;

                    handles.Add((driver, query));
                }

#pragma warning disable CS4014

                Task.Run(async () =>
                {
                    foreach (var (driver, query) in handles)
                    {
                        try
                        {
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


                return await context.ExitAsync();
            });
        }

        private QueryContext CreateQueryContext(DataSource source)
        {
            LogEventSlot slot = _temp.CreateSlot(source);
            QueryContext context = new QueryContext(slot, source);
            return context;
        }

    }
}
