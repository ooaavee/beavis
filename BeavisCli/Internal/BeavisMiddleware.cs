using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
            string method = httpContext.Request.Method;
            PathString path = httpContext.Request.Path;

            bool handled = false;

            if (method == HttpMethods.Get)
            {
                if (path.StartsWithSegments("/beavis/css", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _renderer.RenderCssAsync(httpContext.Response);
                    handled = true;
                }
                else if (path.StartsWithSegments("/beavis/js", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _renderer.RenderJsAsync(httpContext.Response);
                    handled = true;
                }
                else if (path.StartsWithSegments("/beavis", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _renderer.RenderHtmlAsync(httpContext.Response);
                    handled = true;
                }
            }
            else if (method == HttpMethods.Post)
            {
                if (path.StartsWithSegments("/beavis/request", StringComparison.InvariantCultureIgnoreCase))
                {

                    string s = ReadRequest(httpContext.Request);
                    ApplicationExecutionRequest request = JsonConvert.DeserializeObject<ApplicationExecutionRequest>(s);

                    Debugger.Break();

                    handled = true;
                }
            }

            if (!handled)
            {
                await _next(httpContext);
            }
        }

        private static string ReadRequest(HttpRequest request)
        {
            string content;
            using (var stream = request.Body)
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }


    }
}
