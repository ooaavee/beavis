using System;
using BeavisCli;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using Microsoft.AspNetCore.Http;

namespace BeavisLogs.Drivers
{
    public abstract class LogEventOutputRenderer
    {
        public virtual int MaxBlockSize => 50;

        public abstract void OnServingStarted(DataSourceInfo source, HttpContext context, Response response);

        public abstract void Output(DataSourceInfo source, ILogEvent[] events, HttpContext context, Response response);

        public abstract void Output(DataSourceInfo source, Exception[] exceptions, HttpContext context, Response response);

        public abstract void OnLimitReached(HttpContext context, Response response);

        public abstract void OnPollingCompleted(HttpContext context, Response response);
    }
}