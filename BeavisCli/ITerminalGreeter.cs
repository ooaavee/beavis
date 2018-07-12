using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ITerminalGreeter
    {
        void SayGreetings(HttpContext context, WebCliResponse response);
    }
}
