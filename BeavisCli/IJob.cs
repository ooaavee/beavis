using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IJob
    {
        Task RunAsync(HttpContext context, Response response);
    }
}
