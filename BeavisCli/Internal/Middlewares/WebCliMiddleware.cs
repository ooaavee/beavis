using BeavisCli.Internal.Applications;
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
        private readonly IFileStorage _files;
        private readonly WebCliOptions _options;

        public WebCliMiddleware(RequestDelegate next, 
                                ILoggerFactory loggerFactory,
                                WebCliSandbox sandbox, 
                                JobPool jobs, 
                                ITerminalInitializer initializer, 
                                IFileStorage files, 
                                IOptions<WebCliOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<WebCliMiddleware>();
            _sandbox = sandbox;
            _jobs = jobs;
            _initializer = initializer;
            _files = files;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;

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
                _logger.LogInformation($"Started to process a request for a path '{httpContext.Request.Path.ToString()}'.");

                // terminal HTML
                if (IsRequest(_options.Path, HttpMethods.Get))
                {
                    await HandleGetTerminal(httpContext);
                    return;
                }

                // terminal CSS
                if (IsRequest($"{DefaultPath}/content/css", HttpMethods.Get))
                {
                    await HandleGetTerminalCss(httpContext);
                    return;
                }

                // terminal JS
                if (IsRequest($"{DefaultPath}/content/js", HttpMethods.Get))
                {
                    await HandleGetTerminalJs(httpContext);
                    return;
                }

                // initialize terminal
                if (IsRequest($"{DefaultPath}/api/initialize", HttpMethods.Post))
                {
                    await HandleInitializeTerminal(httpContext);
                    return;
                }

                // invoke job
                if (IsRequest($"{DefaultPath}/api/job", HttpMethods.Post))
                {
                    await HandleInvokeJob(httpContext);
                    return;
                }

                // invoke application
                if (IsRequest($"{DefaultPath}/api/request", HttpMethods.Post))
                {
                    await HandleInvokeApplication(httpContext);
                    return;
                }

                // file upload
                if (IsRequest($"{DefaultPath}/api/upload", HttpMethods.Post))
                {
                    await HandleFileUpload(httpContext);
                    return;
                }

                _logger.LogInformation($"Received a request for a path '{httpContext.Request.Path.ToString()}', but unable to find a handler for it.");
            }

            await _next(httpContext);
        }

        private async Task HandleGetTerminal(HttpContext httpContext)
        {
            try
            {
                await WebCliRenderer.RenderHtmlAsync(httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while processing the get terminal html request.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task HandleGetTerminalCss(HttpContext httpContext)
        {
            try
            {
                await WebCliRenderer.RenderCssAsync(httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while processing the get terminal css request.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task HandleGetTerminalJs(HttpContext httpContext)
        {
            try
            {
                await WebCliRenderer.RenderJsAsync(httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while processing the get terminal js request.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task HandleInitializeTerminal(HttpContext httpContext)
        {
            try
            {
                WebCliResponse response = new WebCliResponse(httpContext);

                // prepare tab completion
                response.AddJavaScript(new SetTerminalCompletionDictionary(_sandbox.GetApplications(httpContext).Select(x => x.GetInfo().Name)));

                // set window variables
                WebCliApplicationInfo uploadInfo = WebCliApplicationInfo.Parse<Upload>();
                WebCliOptions.DefaultApplicationBehaviour uploadBehaviour = _options.DefaultApplications[uploadInfo.Name];               
                response.AddJavaScript(new SetUploadEnabled(uploadBehaviour.Enabled));

                if (_options.UseTerminalInitializer)
                {
                    _initializer?.Initialize(httpContext, response);
                }

                await WebCliRenderer.RenderResponseAsync(response, httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while processing the initialize terminal request.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task HandleInvokeJob(HttpContext httpContext)
        {
            try
            {
                WebCliResponse response = new WebCliResponse(httpContext);
                string key = httpContext.Request.Query["key"];
                await _jobs.RunAsync(key, httpContext, response);
                await WebCliRenderer.RenderResponseAsync(response, httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while processing a job.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task HandleInvokeApplication(HttpContext httpContext)
        {
            try
            {
                string body = await ReadBodyAsync(httpContext);
                WebCliRequest request = JsonConvert.DeserializeObject<WebCliRequest>(body);
                WebCliResponse response = new WebCliResponse(httpContext);
                await _sandbox.ExecuteAsync(request, response, httpContext);
                await WebCliRenderer.RenderResponseAsync(response, httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while processing the invoke application request.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task HandleFileUpload(HttpContext httpContext)
        {
            try
            {
                string body = await ReadBodyAsync(httpContext);
                WebCliResponse response = new WebCliResponse(httpContext);
                UploadedFile file = JsonConvert.DeserializeObject<UploadedFile>(body);
                string id = await _files.StoreAsync(file);
                response.WriteInformation("File upload completed, the file ID is:");
                response.WriteInformation(id);
                await WebCliRenderer.RenderResponseAsync(response, httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while processing the file upload request.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task WriteErrorResponseAsync(Exception e, HttpContext httpContext)
        {
            string text = _options.DisplayExceptions ? 
                e.ToString() : 
                "An error occurred. Please check your application logs for more details.";

            httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "text/plain";
            await httpContext.Response.WriteAsync(text, Encoding.UTF8);
        }

        private static async Task<string> ReadBodyAsync(HttpContext httpContext)
        {
            using (Stream stream = httpContext.Request.Body)
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
