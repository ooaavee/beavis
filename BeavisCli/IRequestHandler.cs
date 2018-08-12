using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IRequestHandler
    {
        Task<Response> HandleAsync(Request request, HttpContext httpContext);
    }
}
