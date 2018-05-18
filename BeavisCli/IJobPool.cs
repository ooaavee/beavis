using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IJobPool
    {
        Task ExecuteAsync(string key, HttpContext context, WebCliResponse response);

        string Push(IJob job);
    }
}
