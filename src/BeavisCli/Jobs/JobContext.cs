using System;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Jobs
{
    public class JobContext
    {
        public JobContext(string content, HttpContext context, Response response)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            HttpContext = context ?? throw new ArgumentNullException(nameof(context));
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public string Content { get; }

        public HttpContext HttpContext { get; }

        public Response Response { get; }
    }
}
