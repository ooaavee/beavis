using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ITerminalInitializer
    {
        void Initialize(HttpContext context, WebCliResponse response);
    }
}
