using Microsoft.AspNetCore.Http;

namespace BeavisCli.Services
{
    public interface ITerminalInitializer
    {
        void Initialize(Response response, HttpContext context, bool silent = false);
    }
}
