using BeavisCli;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using BeavisLogs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace BeavisLogs.Drivers
{
    public abstract class LogEventOutputRenderer
    {
        public virtual int MaxBlockSize => 50;

        public abstract void OnPollingCompleted(HttpContext context, Response response);

        public abstract void OnLimitReached(HttpContext context, Response response);

        public abstract void OnServingStarted(DataSourceInfo source, HttpContext context, Response response);

        public abstract void Output(DataSourceInfo source, ILogEvent[] events, HttpContext context, Response response);

        public abstract void Output(DataSourceInfo source, Exception[] exceptions, HttpContext context, Response response);
    }

    public class LogEventTerminalRenderer : LogEventOutputRenderer
    {
        public override void OnPollingCompleted(HttpContext context, Response response)
        {
            response.RenderMode = ResponseRenderMode.Readable;

            response.Messages.Add(ResponseMessage.Success("COMPLETED!111!!!"));
        }

        public override void OnLimitReached(HttpContext context, Response response)
        {
            response.RenderMode = ResponseRenderMode.Readable;

            response.Messages.Add(ResponseMessage.Error("LIMIT REACHED :("));
        }

        public override void OnServingStarted(DataSourceInfo source, HttpContext context, Response response)
        {
            response.RenderMode = ResponseRenderMode.Readable;

            response.Messages.Add(ResponseMessage.Success("Tässä tulee eventtejä sourcelle " + source.Id));
        }

        public override void Output(DataSourceInfo source, ILogEvent[] events, HttpContext context, Response response)
        {
            LogEventFormatter formatter = context.RequestServices.GetRequiredService<LogEventFormatter>();

            response.RenderMode = ResponseRenderMode.Strict;

            foreach (ILogEvent e in events)
            {
                string text = formatter.Format(e);

                // FOR TESTING
                text = source.Id + " " + text;
                     
                ResponseMessage message;

                LogLevel level = e.Level;

                if (level == LogLevel.Error || level == LogLevel.Critical)
                {
                    message = ResponseMessage.Error(text);
                }
                else
                {
                    message = ResponseMessage.Plain(text);
                }

                response.Messages.Add(message);
            }
        }

        public override void Output(DataSourceInfo source, Exception[] exceptions, HttpContext context, Response response)
        {
            throw new NotImplementedException();
        }
    }

    public class LogEventFileRenderer : LogEventOutputRenderer
    {
        public override int MaxBlockSize => int.MaxValue;

        public override void OnPollingCompleted(HttpContext context, Response response)
        {
            // TODO: Tee fileistä ZIPi ja lähetä ne clientielli

            throw new NotImplementedException();
        }

        public override void OnLimitReached(HttpContext context, Response response)
        {
            throw new NotImplementedException();
        }

        public override void OnServingStarted(DataSourceInfo source, HttpContext context, Response response)
        {

        }

        public override void Output(DataSourceInfo source, ILogEvent[] events, HttpContext context, Response response)
        {
            throw new NotImplementedException();
        }

        public override void Output(DataSourceInfo source, Exception[] exceptions, HttpContext context, Response response)
        {
            throw new NotImplementedException();
        }
    }

}