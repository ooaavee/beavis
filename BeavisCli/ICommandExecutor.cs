using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BeavisCli
{
    public interface ICommandExecutor
    {
        Task<Response> ExecuteAsync(JObject requestBody, HttpContext httpContext);
    }
}
