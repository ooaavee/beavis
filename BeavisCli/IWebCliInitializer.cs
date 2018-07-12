using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IWebCliInitializer
    {
        void Initialize(HttpContext context, WebCliResponse response);
    }
}
