using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class BeavisMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApplicationExecutor _executor;
        private readonly WebRenderer _renderer;

        public BeavisMiddleware(RequestDelegate next, ApplicationExecutor executor, WebRenderer renderer)
        {
            _next = next;
            _executor = executor;
            _renderer = renderer;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Method == HttpMethods.Get)
            {
                if (httpContext.Request.Path.Equals("/beavis", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _renderer.RenderHtmlAsync(httpContext.Response);
                    return;
                }

                if (httpContext.Request.Path.Equals("/beavis/css", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _renderer.RenderCssAsync(httpContext.Response);
                    return;
                }

                if (httpContext.Request.Path.Equals("/beavis/js", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _renderer.RenderJsAsync(httpContext.Response);
                    return;
                }
            }
            else if (httpContext.Request.Method == HttpMethods.Post)
            {
                if (httpContext.Request.Path.Equals("/beavis/request", StringComparison.InvariantCultureIgnoreCase))
                {
                    var body = ReadBodyAsText(httpContext.Request);
                    var request = JsonConvert.DeserializeObject<ApplicationExecutionRequest>(body);
                    var response = new ApplicationExecutionResponse();
                    await _executor.HandleAsync(request, response, httpContext);
                    await _renderer.RenderResponseAsync(response, httpContext.Response);
                    return;
                }
            }

            await _next(httpContext);
        }

        private static string ReadBodyAsText(HttpRequest request)
        {
            using (var stream = request.Body)
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

    }
}
