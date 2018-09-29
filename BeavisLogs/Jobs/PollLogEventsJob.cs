using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using BeavisCli;
using BeavisCli.JavaScriptStatements;
using BeavisLogs.Drivers;
using BeavisLogs.Models.Logs;
using BeavisLogs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeavisLogs.Jobs
{
    public class PollLogEventsJob : IJob
    {
        private const int PopCount = 25;

        private readonly string _slotKey;

        public PollLogEventsJob(string slotKey)
        {
            _slotKey = slotKey;
        }

        public Task RunAsync(JobContext context)
        {
            LogEventTempStorage storage = context.HttpContext.RequestServices.GetRequiredService<LogEventTempStorage>();

            if (storage.TryGetSlot(_slotKey, out LogEventSlot slot))
            {
                bool pending = EnumerateLogEvents(slot, context);

                if (pending)
                {
                    ScheduleNextPoll(context);
                }
                else
                {
                    OnPollingCompleted(context);
                }
            }
            else
            {
                OnNoSlotFound(context);
            }

            return Task.CompletedTask;
        }

        private bool EnumerateLogEvents(LogEventSlot slot, JobContext context)
        {
            LogEventRenderer renderer = context.HttpContext.RequestServices.GetRequiredService<LogEventRenderer>();

            context.Response.RenderMode = ResponseRenderMode.Strict;

            List<bool> pendings = new List<bool>();

            foreach (LogEventSlot.SlotEnumerator enumerator in slot.Enumerators)
            {
                ILogEvent[] events = enumerator.Pop(PopCount, out Exception[] exceptions, out bool pending);

                pendings.Add(pending);

                foreach (ILogEvent e in events)
                {
                    OutputEvent(e, context, renderer);
                }

                foreach (Exception ex in exceptions)
                {
                    OutputException(ex, context, renderer);
                }
            }

            if (!pendings.Any(x => x))
            {
                if (slot.IsLimitReached)
                {
                    OnLimitReached(context);
                }
                return false;
            }

            return true;
        }

        private void ScheduleNextPoll(JobContext context)
        {
            // this will be invoked just before we are sending the response
            context.Response.Sending += (sender, args) =>
            {
                // push a new job into the pool and add a JavaScript statement that
                // begins the job on the client-side
                IJobPool pool = context.HttpContext.RequestServices.GetRequiredService<IJobPool>();
                IJob job = new PollLogEventsJob(_slotKey);
                string key = pool.Push(job);
                IJavaScriptStatement js = new BeginJob(key);
                context.Response.Statements.Add(js);
            };
        }

        private void OnNoSlotFound(JobContext context)
        {
            Debugger.Break();
        }

        private void OnPollingCompleted(JobContext context)
        {
            context.Response.Messages.Add(ResponseMessage.Success("COMPLETED"));
        }

        private void OnLimitReached(JobContext context)
        {
            context.Response.Messages.Add(ResponseMessage.Error("LIMIT REACHED!!!11!!"));
        }

        private static void OutputEvent(ILogEvent e, JobContext context, LogEventRenderer renderer)
        {
            string output = renderer.Render(e);

            ResponseMessage message;

            LogLevel level = e.Level;

            if (level == LogLevel.Error || level == LogLevel.Critical)
            {
                message = ResponseMessage.Error(output);
            }
            else
            {
                message = ResponseMessage.Plain(output);
            }

            context.Response.Messages.Add(message);
        }

        private static void OutputException(Exception ex, JobContext context, LogEventRenderer renderer)
        {
            // TODO

        }



    }
}
