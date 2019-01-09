using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli
{
    public static class JobScheduler
    {
        public static void New(IJob job, Response response, HttpContext context)
        {
            // this will be invoked just before we are sending the response
            response.Sending += (sender, args) =>
            {
                // push a new job into the pool and add a JavaScript statement that
                // begins the job on the client-side
                IJobPool pool = context.RequestServices.GetRequiredService<IJobPool>();
                string key = pool.Push(job);
                IJavaScriptStatement js = new BeginJob(key);
                response.Statements.Add(js);
            };
        }
    }
}
