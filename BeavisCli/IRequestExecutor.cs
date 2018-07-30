using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IRequestExecutor
    {
        Task ExecuteAsync(Request request, Response response, HttpContext httpContext);
    }
}
