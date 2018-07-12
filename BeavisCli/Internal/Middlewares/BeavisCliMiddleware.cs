using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BeavisCli.Internal.Middlewares
{
    internal class BeavisCliMiddleware
    {
        private const string DefaultPath = "/beavis-cli";

        private readonly RequestDelegate _next;
        private readonly WebCliSandbox _sandbox;
        private readonly WebRenderer _renderer;
        private readonly IJobPool _jobPool;
        private readonly ITerminalInitializer _initializer;
        private readonly ITerminalGreeter _greeter;
        private readonly IFileUploadStorage _fileUploadStorage;
        private readonly string _path;

        public BeavisCliMiddleware(
            RequestDelegate next, 
            WebCliSandbox sandbox, 
            WebRenderer renderer, 
            IJobPool jobPool, 
            ITerminalInitializer initializer, 
            ITerminalGreeter greeter,
            IFileUploadStorage fileUploadStorage, 
            IOptions<WebCliOptions> options)
        {
            _next = next;
            _sandbox = sandbox;
            _renderer = renderer;
            _jobPool = jobPool;
            _initializer = initializer;
            _greeter = greeter;
            _fileUploadStorage = fileUploadStorage;
            _path = options.Value.Path;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsValidPathStart(context))
            {
                // Terminal HTML
                if (Match(_path, HttpMethods.Get, context))
                {
                    await _renderer.RenderHtmlAsync(context);
                    return;
                }

                // Terminal CSS
                if (Match($"{DefaultPath}/content/css", HttpMethods.Get, context))
                {
                    await _renderer.RenderCssAsync(context);
                    return;
                }

                // Terminal JS
                if (Match($"{DefaultPath}/content/js", HttpMethods.Get, context))
                {
                    await _renderer.RenderJsAsync(context);
                    return;
                }

                // Initialize terminal
                if (Match($"{DefaultPath}/api/initialize", HttpMethods.Post, context))
                {
                    var response = new WebCliResponse(context);
                    _initializer?.Initialize(context, response);
                    _greeter?.SayGreetings(context, response);
                    await _renderer.RenderResponseAsync(response, context);
                    return;
                }

                // Invoke job by key
                if (Match($"{DefaultPath}/api/job", HttpMethods.Post, context))
                {
                    string key = context.Request.Query["key"];
                    WebCliResponse response = new WebCliResponse(context);
                    await _jobPool.ExecuteAsync(key, context, response);
                    await _renderer.RenderResponseAsync(response, context);
                    return;
                }

                // Invoke application
                if (Match($"{DefaultPath}/api/request", HttpMethods.Post, context))
                {
                    string body = GetRequestBodyAsText(context);
                    WebCliRequest request = JsonConvert.DeserializeObject<WebCliRequest>(body);
                    WebCliResponse response = new WebCliResponse(context);
                    await _sandbox.ExecuteAsync(request, response, context);
                    await _renderer.RenderResponseAsync(response, context);
                    return;
                }

                // File upload
                if (Match($"{DefaultPath}/api/upload", HttpMethods.Post, context))
                {
                    string body = GetRequestBodyAsText(context);
                    WebCliResponse response = new WebCliResponse(context);
                    if (_fileUploadStorage != null)
                    {
                        UploadedFile file = JsonConvert.DeserializeObject<UploadedFile>(body);
                        await _fileUploadStorage.UploadAsync(file, response);
                    }
                    await _renderer.RenderResponseAsync(response, context);
                    return;
                }
            }

            await _next(context);
        }

        private bool IsValidPathStart(HttpContext context)
        {
            if (context.Request.Path.HasValue)
            {
                if (context.Request.Path.StartsWithSegments(_path, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                if (context.Request.Path.StartsWithSegments(DefaultPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Match(string path, string httpMethod, HttpContext context)
        {
            return context.Request.Method == httpMethod && context.Request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string GetRequestBodyAsText(HttpContext context)
        {
            using (Stream stream = context.Request.Body)
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
