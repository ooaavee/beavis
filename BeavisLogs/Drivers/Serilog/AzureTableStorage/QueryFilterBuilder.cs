using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeavisLogs.Models.Logs;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class QueryFilterBuilder
    {

        public QueryFilter Build(QueryParameters parameters)
        {
            var filter = new QueryFilter();
            filter.TableQuery = TableQuery(parameters);
            filter.PostQueryFilters.AddRange(PostQueryFilters(parameters));
            return filter;
        }

        private TableQuery<LogEventTableEntity> TableQuery(QueryParameters parameters)
        {
            var tableQuery = new TableQuery<LogEventTableEntity>();

            // TODO: Tee päivämäärärajaus!

            return tableQuery;
        }

        private IEnumerable<Predicate<ILogEvent>> PostQueryFilters(QueryParameters parameters)
        {
            // log levels
            if (parameters.Levels.Any())
            {
                yield return delegate (ILogEvent e)
                {
                    return parameters.Levels.Contains(e.Level);
                };
            }

            // regex for message and exception text
            if (parameters.Regex != null)
            {
                yield return delegate(ILogEvent e)
                {
                    if (e.Message != null)
                    {
                        if (Regex.IsMatch(e.Message, parameters.Regex))
                        {
                            return true;
                        }
                    }

                    if (e.Exception != null)
                    {
                        if (Regex.IsMatch(e.Exception, parameters.Regex))
                        {
                            return true;
                        }
                    }

                    return false;
                };
            }

        }
    }
}
