using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class BeavisCliMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApplicationExecutor _executor;
        private readonly WebRenderer _renderer;
        private readonly BeavisCliOptions _options;

        public BeavisCliMiddleware(RequestDelegate next, ApplicationExecutor executor, WebRenderer renderer, IOptions<BeavisCliOptions> options)
        {
            _next = next;
            _executor = executor;
            _renderer = renderer;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (IsPath("/beavis-cli", HttpMethods.Get, httpContext))
            {
                await _renderer.RenderHtmlAsync(httpContext.Response);
                return;
            }

            if (IsPath("/beavis-cli/content/css", HttpMethods.Get, httpContext))
            {
                await _renderer.RenderCssAsync(httpContext.Response);
                return;
            }

            if (IsPath("/beavis-cli/content/js", HttpMethods.Get, httpContext))
            {
                await _renderer.RenderJsAsync(httpContext.Response);
                return;
            }

            if (IsPath("/beavis-cli/api/welcome", HttpMethods.Post, httpContext))
            {
                var response = new ApplicationExecutionResponse();
                if (_options.WelcomeHandler != null)
                {
                    _options.WelcomeHandler.SayWelcome(response);
                }
                await _renderer.RenderResponseAsync(response, httpContext.Response);
                return;
            }

            if (IsPath("/beavis-cli/api/request", HttpMethods.Post, httpContext))
            {
                var body = ReadBodyAsText(httpContext.Request);
                var request = JsonConvert.DeserializeObject<ApplicationExecutionRequest>(body);
                var response = new ApplicationExecutionResponse();
                await _executor.HandleAsync(request, response, httpContext);
                await _renderer.RenderResponseAsync(response, httpContext.Response);
                return;
            }


            await _next(httpContext);
        }

        private static bool IsPath(string path, string method, HttpContext httpContext)
        {
            if (httpContext.Request.Path.HasValue)
            {
                if (httpContext.Request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (httpContext.Request.Method == method)
                    {
                        return true;
                    }
                }
            }
            return false;
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
