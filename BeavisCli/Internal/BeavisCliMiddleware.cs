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
        private readonly BeavisCliSandbox _sandbox;
        private readonly WebRenderer _renderer;
        private readonly WebCliOptions _options;

        public BeavisCliMiddleware(RequestDelegate next, BeavisCliSandbox sandbox, WebRenderer renderer, IOptions<WebCliOptions> options)
        {
            _next = next;
            _sandbox = sandbox;
            _renderer = renderer;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsPath("/beavis-cli", HttpMethods.Get, context))
            {
                await _renderer.RenderHtmlAsync(context);
                return;
            }

            if (IsPath("/beavis-cli/content/css", HttpMethods.Get, context))
            {
                await _renderer.RenderCssAsync(context);
                return;
            }

            if (IsPath("/beavis-cli/content/js", HttpMethods.Get, context))
            {
                await _renderer.RenderJsAsync(context);
                return;
            }

            if (IsPath("/beavis-cli/api/initialize", HttpMethods.Post, context))
            {
                var response = new WebCliResponse();
                _options.TerminalInitializer?.Initialize(context, response);
                await _renderer.RenderResponseAsync(response, context);
                return;
            }

            if (IsPath("/beavis-cli/api/request", HttpMethods.Post, context))
            {
                var body = GetRequestBodyAsText(context);
                var request = JsonConvert.DeserializeObject<WebCliRequest>(body);
                var response = new WebCliResponse();
                await _sandbox.ExecuteAsync(request, response, context);
                await _renderer.RenderResponseAsync(response, context);
                return;
            }

            await _next(context);
        }

        private static bool IsPath(string path, string httpMethod, HttpContext context)
        {
            return context.Request.Method == httpMethod &&
                   context.Request.Path.HasValue &&
                   context.Request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string GetRequestBodyAsText(HttpContext context)
        {
            using (var stream = context.Request.Body)
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

    }
}
