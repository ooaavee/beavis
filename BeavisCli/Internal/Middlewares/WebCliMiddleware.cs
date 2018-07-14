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
        private readonly IWebCliInitializer _initializer;
        private readonly IUploadStorage _uploadStorage;
        private readonly WebCliOptions _options;

        public WebCliMiddleware(RequestDelegate next, 
                                ILoggerFactory loggerFactory,
                                WebCliSandbox sandbox, 
                                JobPool jobs, 
                                IWebCliInitializer initializer, 
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
            bool IsWebCliPath()
            {
                return context.Request.Path.StartsWithSegments(_options.Path, StringComparison.InvariantCultureIgnoreCase) ||
                       context.Request.Path.StartsWithSegments(DefaultPath, StringComparison.InvariantCultureIgnoreCase);
            }

            bool IsRequest(string path, string method)
            {
                return context.Request.Method == method &&
                       context.Request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
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

                _initializer?.Initialize(context, response);

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
                throw new NotImplementedException("Huuhaaaa!!!");

                string body = GetRequestBodyAsText(context);
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
                string body = GetRequestBodyAsText(context);
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
            StringBuilder text = new StringBuilder();

            if (_options.DisplayExceptions)
            {
                text.AppendLine(e.ToString());
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(text.ToString(), Encoding.UTF8);
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
