using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Services
{
    public interface IRequestHandler
    {
        Task<Response> HandleAsync(Request request, HttpContext context);
    }
}
