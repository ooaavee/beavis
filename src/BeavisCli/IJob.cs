using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IJob
    {
        Task RunAsync(JobContext context);
    }
}
