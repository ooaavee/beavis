using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ITerminalInitializer
    {
        void Initialize(Response response, HttpContext httpContext, bool silent = false);
    }
}
