using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    internal interface IJobPool
    {
        /// <summary>
        /// Adds a job and returns its identifier. 
        /// </summary>
        string Push(IJob job);

        /// <summary>
        /// Runs the job.
        /// </summary>
        Task RunAsync(string key, JobContext context);
    }
}