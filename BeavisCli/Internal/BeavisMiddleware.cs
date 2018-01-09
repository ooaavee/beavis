using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Internal
{
    internal class BeavisMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestHandler _requestHandler;
        private readonly WebTerminalRenderer _renderer;
        private readonly Dictionary<string, Func<HttpContext, Task>> _funcs = new Dictionary<string, Func<HttpContext, Task>>();

        public BeavisMiddleware(RequestDelegate next, RequestHandler requestHandler, WebTerminalRenderer renderer)
        {
            _next = next;
            _requestHandler = requestHandler;
            _renderer = renderer;

            _funcs["/beavis"] = HandleGetHtmlAsync;
            _funcs["/beavis/css"] = HandleGetCssAsync;
            _funcs["/beavis/js"] = HandleGetJsAsync;
            _funcs["/beavis/request"] = HandleRequestAsync;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.HasValue)
            {
                string path = httpContext.Request.Path.Value.ToLowerInvariant();

                if (_funcs.TryGetValue(path, out var func))
                {
                    await func(httpContext);
                    return;
                }
            }

            await _next(httpContext);
        }


        private async Task HandleGetCssAsync(HttpContext httpContext)
        {
            string css = _renderer.GetTerminalCss();
            byte[] content = Encoding.UTF8.GetBytes(css);

            httpContext.Response.ContentType = "text/css";
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

            await httpContext.Response.Body.WriteAsync(content, 0, content.Length);
        }

        private async Task HandleGetJsAsync(HttpContext httpContext)
        {
            string js = _renderer.GetTerminalJs();
            byte[] content = Encoding.UTF8.GetBytes(js);

            httpContext.Response.ContentType = "application/javascript";
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

            await httpContext.Response.Body.WriteAsync(content, 0, content.Length);
        }

        private async Task HandleGetHtmlAsync(HttpContext httpContext)
        {
            string html = _renderer.GetTerminalHtml();
            byte[] content = Encoding.UTF8.GetBytes(html);

            httpContext.Response.ContentType = "text/html";
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;

            await httpContext.Response.Body.WriteAsync(content, 0, content.Length);
        }

        private async Task HandleRequestAsync(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

    }
}
