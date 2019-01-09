using BeavisLogs.Models.Logs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class Query
    {
        public Query(TableQuery<LogEventTableEntity> tableQuery, IEnumerable<Predicate<ILogEvent>> filters)
        {
            TableQuery = tableQuery;
            Filters = filters.ToArray();
        }

        public TableQuery<LogEventTableEntity> TableQuery { get; }

        public Predicate<ILogEvent>[] Filters { get; }

        public bool PassFilters(ILogEvent e)
        {
            return Filters.All(filter => filter(e));
        }
    }
}