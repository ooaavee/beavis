using System;
using System.Collections.Generic;
using BeavisLogs.Models.Logs;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class QueryFilter
    {
        /// <summary>
        /// Azure Table Storage query
        /// </summary>
        public TableQuery<LogEventTableEntity> TableQuery { get; set; }

        /// <summary>
        /// Post query predicates
        /// </summary>
        public List<Predicate<ILogEvent>> PostQueryFilters { get; } = new List<Predicate<ILogEvent>>();
    }
}