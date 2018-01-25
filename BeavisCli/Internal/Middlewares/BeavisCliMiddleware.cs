using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BeavisCli.Internal.Middlewares
{
    internal class BeavisCliMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebCliSandbox _sandbox;
        private readonly WebRenderer _renderer;
        private readonly JobManager _jobManager;
        private readonly ITerminalInitializer _initializer;
        private readonly IFileUploadStorage _fileUploadStorage;

        public BeavisCliMiddleware(RequestDelegate next, WebCliSandbox sandbox, WebRenderer renderer, JobManager jobManager, ITerminalInitializer initializer, IFileUploadStorage fileUploadStorage)
        {
            _next = next;
            _sandbox = sandbox;
            _renderer = renderer;
            _jobManager = jobManager;
            _initializer = initializer;
            _fileUploadStorage = fileUploadStorage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsValidPathStart(context))
            {
                await _next(context);
                return;
            }

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
                var response = new WebCliResponse(context);
                _initializer?.Initialize(context, response);
                await _renderer.RenderResponseAsync(response, context);
                return;
            }

            if (IsPath("/beavis-cli/api/job", HttpMethods.Post, context))
            {
                var key = context.Request.Query["key"];
                var response = new WebCliResponse(context);
                await _jobManager.ExecuteAsync(key, context, response);
                await _renderer.RenderResponseAsync(response, context);
                return;
            }

            if (IsPath("/beavis-cli/api/request", HttpMethods.Post, context))
            {
                var body = GetRequestBodyAsText(context);
                var request = JsonConvert.DeserializeObject<WebCliRequest>(body);
                var response = new WebCliResponse(context);
                await _sandbox.ExecuteAsync(request, response, context);
                await _renderer.RenderResponseAsync(response, context);
                return;
            }

            if (IsPath("/beavis-cli/api/upload", HttpMethods.Post, context))
            {
                var body = GetRequestBodyAsText(context);
                var response = new WebCliResponse(context);
                if (_fileUploadStorage != null)
                {
                    var file = JsonConvert.DeserializeObject<UploadedFile>(body);
                    await _fileUploadStorage.UploadAsync(file, response);
                }
                await _renderer.RenderResponseAsync(response, context);
                return;
            }

            await _next(context);
        }

        private static bool IsValidPathStart(HttpContext context)
        {
            return context.Request.Path.HasValue &&
                   context.Request.Path.StartsWithSegments("/beavis-cli", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsPath(string path, string httpMethod, HttpContext context)
        {
            return context.Request.Method == httpMethod &&
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
