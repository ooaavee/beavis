using BeavisLogs.Models.Logs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class QueryFilter
    {
        private readonly Predicate<ILogEvent>[] _filters;

        public QueryFilter(TableQuery<LogEventTableEntity> query, IEnumerable<Predicate<ILogEvent>> filters)
        {
            Query = query;
            _filters = filters.ToArray();
        }

        public TableQuery<LogEventTableEntity> Query { get; }

        public bool Success(ILogEvent e)
        {
            return _filters.All(filter => filter(e));
        }
    }
}