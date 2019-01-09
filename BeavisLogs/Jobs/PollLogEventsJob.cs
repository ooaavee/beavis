using BeavisCli;
using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using BeavisLogs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BeavisLogs.Jobs
{
    public class PollLogEventsJob : IJob
    {
        private readonly string _slotKey;

        public PollLogEventsJob(string slotKey)
        {
            _slotKey = slotKey;
        }

        public Task RunAsync(JobContext context)
        {
            LogEventTempStorage storage = context.HttpContext.RequestServices.GetRequiredService<LogEventTempStorage>();

            if (storage.TryGet(_slotKey, out LogEventSlot slot))
            {
                bool ready = slot.IsReadyToServe();

                bool more = false;

                if (ready)
                {
                    more = slot.Serve(context.HttpContext, context.Response);
                }

                if (!ready || more)
                {
                    // schedule next poll
                    IJob job = new PollLogEventsJob(_slotKey);
                    JobScheduler.New(job, context.Response, context.HttpContext);
                }
                else
                {
                    storage.Remove(_slotKey);
                }
            }
            else
            {
                OnNoSlotFound(context);
            }

            return Task.CompletedTask;
        }

        private static void OnNoSlotFound(JobContext context)
        {
            // TODO: Kirjoita tässä viesti terminaaliin riippumatta siitä onko mikä renderer käytössä!!!
            Debugger.Break();
        }
    }
}
