using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Middlewares
{
    internal class WebCliMiddleware
    {
        private const string DefaultPath = "/beavis-cli";

        private readonly RequestDelegate _next;
        private readonly WebCliSandbox _sandbox;
        private readonly JobPool _jobs;
        private readonly IWebCliInitializer _initializer;
        private readonly IFileUploadStorage _fileUploadStorage;
        private readonly WebCliOptions _options;

        public WebCliMiddleware(
            RequestDelegate next,
            WebCliSandbox sandbox,
            JobPool jobs,
            IWebCliInitializer initializer,
            IFileUploadStorage fileUploadStorage,
            IOptions<WebCliOptions> options)
        {
            _next = next;
            _sandbox = sandbox;
            _jobs = jobs;
            _initializer = initializer;
            _fileUploadStorage = fileUploadStorage;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsValidPathStart(context))
            {
                // terminal HTML
                if (Match(_options.Path, HttpMethods.Get, context))
                {
                    await HandleGetTerminal(context);
                    return;
                }

                // terminal CSS
                if (Match($"{DefaultPath}/content/css", HttpMethods.Get, context))
                {
                    await HandleGetTerminalCss(context);
                    return;
                }

                // terminal JS
                if (Match($"{DefaultPath}/content/js", HttpMethods.Get, context))
                {
                    await HandleGetTerminalJs(context);
                    return;
                }

                // initialize terminal
                if (Match($"{DefaultPath}/api/initialize", HttpMethods.Post, context))
                {
                    await HandleInitializeTerminal(context);
                    return;
                }

                // invoke job
                if (Match($"{DefaultPath}/api/job", HttpMethods.Post, context))
                {
                    await HandleInvokeJob(context);
                    return;
                }

                // invoke application
                if (Match($"{DefaultPath}/api/request", HttpMethods.Post, context))
                {
                    await HandleInvokeApplication(context);
                    return;
                }

                // file upload
                if (Match($"{DefaultPath}/api/upload", HttpMethods.Post, context))
                {
                    await HandleFileUpload(context);
                    return;
                }
            }

            await _next(context);
        }

        private async Task HandleGetTerminal(HttpContext context)
        {
            await WebCliRenderer.RenderHtmlAsync(context);
        }

        private async Task HandleGetTerminalCss(HttpContext context)
        {
            await WebCliRenderer.RenderCssAsync(context);
        }

        private async Task HandleGetTerminalJs(HttpContext context)
        {
            await WebCliRenderer.RenderJsAsync(context);
        }

        private async Task HandleInitializeTerminal(HttpContext context)
        {
            var response = new WebCliResponse(context);

            // prepare tab completion
            response.AddJavaScript(new SetTerminalCompletionDictionary(_sandbox.GetApplications(context).Select(x => x.Meta().Name)));

            // set variables
            response.AddJavaScript(new SetUploadEnabled(_options.EnableFileUpload));

            _initializer?.Initialize(context, response);

            await WebCliRenderer.RenderResponseAsync(response, context);
        }

        private async Task HandleInvokeJob(HttpContext context)
        {
            string key = context.Request.Query["key"];
            WebCliResponse response = new WebCliResponse(context);
            await _jobs.ExecuteAsync(key, context, response);
            await WebCliRenderer.RenderResponseAsync(response, context);
        }

        private async Task HandleInvokeApplication(HttpContext context)
        {
            string body = GetRequestBodyAsText(context);
            WebCliRequest request = JsonConvert.DeserializeObject<WebCliRequest>(body);
            WebCliResponse response = new WebCliResponse(context);
            await _sandbox.ExecuteAsync(request, response, context);
            await WebCliRenderer.RenderResponseAsync(response, context);
        }

        private async Task HandleFileUpload(HttpContext context)
        {
            string body = GetRequestBodyAsText(context);
            WebCliResponse response = new WebCliResponse(context);
            if (_fileUploadStorage != null)
            {
                UploadedFile file = JsonConvert.DeserializeObject<UploadedFile>(body);
                await _fileUploadStorage.OnFileUploadedAsync(file, response);
            }
            await WebCliRenderer.RenderResponseAsync(response, context);
        }

        private bool IsValidPathStart(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(_options.Path, StringComparison.InvariantCultureIgnoreCase) ||
                   context.Request.Path.StartsWithSegments(DefaultPath, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool Match(string path, string httpMethod, HttpContext context)
        {
            return context.Request.Method == httpMethod &&
                   context.Request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
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
