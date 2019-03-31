using System;
using BeavisCli.JsInterop;
using BeavisCli.JsInterop.Statements;
using BeavisCli.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Jobs
{
    public static class JobScheduler
    {
        public static void Schedule(IJob job, Response response, HttpContext context)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // This will be invoked just before we are sending the response:
            // Push a new job into the pool and add a JavaScript statement that
            // begins the job on the client-side.
            response.Sending += (sender, args) =>
            {
                IJobPool pool = context.RequestServices.GetRequiredService<IJobPool>();
                string key = pool.Push(job);
                IStatement statement = new BeginJob(key);
                response.Statements.Add(statement.GetJs());
            };
        }
    }
}
