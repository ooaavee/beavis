using System.Threading.Tasks;

namespace BeavisCli.Jobs
{
    public interface IJob
    {
        Task RunAsync(JobContext context);
    }
}
