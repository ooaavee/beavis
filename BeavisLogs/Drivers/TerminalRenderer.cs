using System;
using BeavisCli;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using BeavisLogs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeavisLogs.Drivers.Renderers
{
    public class TerminalRenderer : LogEventOutputRenderer
    {
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
            LogEventFormatter formatter = context.RequestServices.GetRequiredService<LogEventFormatter>();

            throw new NotImplementedException();
        }

        public override void OnLimitReached(HttpContext context, Response response)
        {
            response.RenderMode = ResponseRenderMode.Readable;

            response.Messages.Add(ResponseMessage.Error("LIMIT REACHED :("));
        }

        public override void OnPollingCompleted(HttpContext context, Response response)
        {
            response.RenderMode = ResponseRenderMode.Readable;

            response.Messages.Add(ResponseMessage.Success("COMPLETED!111!!!"));
        }
    }
}