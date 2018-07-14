using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Middlewares
{
    internal class WebCliMiddleware
    {
        private const string DefaultPath = "/beavis-cli";

        private readonly RequestDelegate _next;
        private readonly ILogger<WebCliMiddleware> _logger;
        private readonly WebCliSandbox _sandbox;
        private readonly JobPool _jobs;
        private readonly ITerminalInitializer _initializer;
        private readonly IUploadStorage _uploadStorage;
        private readonly WebCliOptions _options;

        public WebCliMiddleware(RequestDelegate next, 
                                ILoggerFactory loggerFactory,
                                WebCliSandbox sandbox, 
                                JobPool jobs, 
                                ITerminalInitializer initializer, 
                                IUploadStorage uploadStorage, 
                                IOptions<WebCliOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<WebCliMiddleware>();
            _sandbox = sandbox;
            _jobs = jobs;
            _initializer = initializer;
            _uploadStorage = uploadStorage;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            HttpRequest request = context.Request;

            bool IsWebCliPath()
            {
                return request.Path.StartsWithSegments(_options.Path, StringComparison.InvariantCultureIgnoreCase) ||
                       request.Path.StartsWithSegments(DefaultPath, StringComparison.InvariantCultureIgnoreCase);
            }

            bool IsRequest(string path, string method)
            {
                return request.Method == method &&
                       request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
            }

            if (IsWebCliPath())
            {
                _logger.LogInformation($"Started to process WebCli request for a path '{context.Request.Path.ToString()}'.");

                // terminal HTML
                if (IsRequest(_options.Path, HttpMethods.Get))
                {
                    await HandleGetTerminal(context);
                    return;
                }

                // terminal CSS
                if (IsRequest($"{DefaultPath}/content/css", HttpMethods.Get))
                {
                    await HandleGetTerminalCss(context);
                    return;
                }

                // terminal JS
                if (IsRequest($"{DefaultPath}/content/js", HttpMethods.Get))
                {
                    await HandleGetTerminalJs(context);
                    return;
                }

                // initialize terminal
                if (IsRequest($"{DefaultPath}/api/initialize", HttpMethods.Post))
                {
                    await HandleInitializeTerminal(context);
                    return;
                }

                // invoke job
                if (IsRequest($"{DefaultPath}/api/job", HttpMethods.Post))
                {
                    await HandleInvokeJob(context);
                    return;
                }

                // invoke application
                if (IsRequest($"{DefaultPath}/api/request", HttpMethods.Post))
                {
                    await HandleInvokeApplication(context);
                    return;
                }

                // file upload
                if (IsRequest($"{DefaultPath}/api/upload", HttpMethods.Post))
                {
                    await HandleFileUpload(context);
                    return;
                }

                _logger.LogInformation($"Received a WebCli request for a path '{context.Request.Path.ToString()}', but unable to find a handler for it.");
            }

            await _next(context);
        }

        private async Task HandleGetTerminal(HttpContext context)
        {
            try
            {
                await WebCliRenderer.RenderHtmlAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred inside the '{nameof(HandleGetTerminal)}'.", e);
                await WriteErrorResponseAsync(e, context);
            }
        }

        private async Task HandleGetTerminalCss(HttpContext context)
        {
            try
            {
                await WebCliRenderer.RenderCssAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred inside the '{nameof(HandleGetTerminalCss)}'.", e);
                await WriteErrorResponseAsync(e, context);
            }
        }

        private async Task HandleGetTerminalJs(HttpContext context)
        {
            try
            {
                await WebCliRenderer.RenderJsAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred inside the '{nameof(HandleGetTerminalJs)}'.", e);
                await WriteErrorResponseAsync(e, context);
            }
        }

        private async Task HandleInitializeTerminal(HttpContext context)
        {
            try
            {
                WebCliResponse response = new WebCliResponse(context);

                // prepare tab completion
                response.AddJavaScript(new SetTerminalCompletionDictionary(_sandbox.GetApplications(context).Select(x => x.Meta().Name)));

                // set window variables
                response.AddJavaScript(new SetUploadEnabled(_options.EnableFileUpload));

                if (_options.UseTerminalInitializer)
                {
                    _initializer?.Initialize(context, response);
                }

                await WebCliRenderer.RenderResponseAsync(response, context);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred inside the '{nameof(HandleInitializeTerminal)}'.", e);
                await WriteErrorResponseAsync(e, context);
            }
        }

        private async Task HandleInvokeJob(HttpContext context)
        {
            try
            {
                WebCliResponse response = new WebCliResponse(context);
                string key = context.Request.Query["key"];
                await _jobs.ExecuteAsync(key, context, response);
                await WebCliRenderer.RenderResponseAsync(response, context);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred inside the '{nameof(HandleInvokeJob)}'.", e);
                await WriteErrorResponseAsync(e, context);
            }
        }

        private async Task HandleInvokeApplication(HttpContext context)
        {
            try
            {
                string body = await ReadBodyAsync(context);
                WebCliRequest request = JsonConvert.DeserializeObject<WebCliRequest>(body);
                WebCliResponse response = new WebCliResponse(context);
                await _sandbox.ExecuteAsync(request, response, context);
                await WebCliRenderer.RenderResponseAsync(response, context);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred inside the '{nameof(HandleInvokeApplication)}'.", e);
                await WriteErrorResponseAsync(e, context);
            }
        }

        private async Task HandleFileUpload(HttpContext context)
        {
            try
            {
                string body = await ReadBodyAsync(context);
                WebCliResponse response = new WebCliResponse(context);
                if (_uploadStorage != null)
                {
                    UploadedFile file = JsonConvert.DeserializeObject<UploadedFile>(body);
                    await _uploadStorage.OnFileUploadedAsync(file, response);
                }
                await WebCliRenderer.RenderResponseAsync(response, context);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred inside the '{nameof(HandleFileUpload)}'.", e);
                await WriteErrorResponseAsync(e, context);
            }
        }

        private async Task WriteErrorResponseAsync(Exception e, HttpContext context)
        {
            string text = _options.DisplayExceptions ? 
                e.ToString() : 
                "An error occurred. Please check your application logs for more details.";

            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(text, Encoding.UTF8);
        }

        private static async Task<string> ReadBodyAsync(HttpContext context)
        {
            using (Stream stream = context.Request.Body)
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string body = await reader.ReadToEndAsync();
                    return body;
                }
            }
        }
    }
}
