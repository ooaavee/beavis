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
        /// Finds a job from the job pool by its identifier and removes it from the job pool.
        /// </summary>
        Task RunAsync(string key, HttpContext context, Response response);
    }
}