using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public class JobContext
    {
        public JobContext(string content, HttpContext httpContext, Response response)
        {
            Content = content;
            HttpContext = httpContext;
            Response = response;
        }

        public string Content { get; }

        public HttpContext HttpContext { get; }

        public Response Response { get; }
    }
}
