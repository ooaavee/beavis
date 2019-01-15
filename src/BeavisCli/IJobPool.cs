using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IJobPool
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