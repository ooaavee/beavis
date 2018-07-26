using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class WebCliMiddleware
    {
        private const string DefaultPath = "/beaviscli";

        private readonly RequestDelegate _next;
        private readonly ILogger<WebCliMiddleware> _logger;
        private readonly WebCliSandbox _sandbox;
        private readonly JobPool _jobs;
        private readonly ITerminalBehaviour _behaviour;
        private readonly IFileStorage _files;
        private readonly WebCliOptions _options;

        public WebCliMiddleware(RequestDelegate next, 
                                ILoggerFactory loggerFactory,
                                WebCliSandbox sandbox, 
                                JobPool jobs, 
                                ITerminalBehaviour behaviour, 
                                IFileStorage files, 
                                IOptions<WebCliOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<WebCliMiddleware>();
            _sandbox = sandbox;
            _jobs = jobs;
            _behaviour = behaviour;
            _files = files;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            HttpRequest request = context.Request;

            if (!IsPotentialMatch(request))
            {
                await _next(context);
                return;
            }

            _logger.LogInformation($"Started to process a request for a path '{request.Path.ToString()}'.");

            // terminal HTML
            if (Match(_options.Path, HttpMethods.Get, request))
            {
                await HandleGetTerminal(context);
                return;
            }

            // terminal CSS
            if (Match($"{DefaultPath}/content/css", HttpMethods.Get, request))
            {
                await HandleGetTerminalCss(context);
                return;
            }

            // terminal JS
            if (Match($"{DefaultPath}/content/js", HttpMethods.Get, request))
            {
                await HandleGetTerminalJs(context);
                return;
            }

            // initialize terminal
            if (Match($"{DefaultPath}/api/initialize", HttpMethods.Post, request))
            {
                await HandleInitializeTerminal(context);
                return;
            }

            // invoke job
            if (Match($"{DefaultPath}/api/job", HttpMethods.Post, request))
            {
                await HandleInvokeJob(context);
                return;
            }

            // invoke command
            if (Match($"{DefaultPath}/api/request", HttpMethods.Post, request))
            {
                await HandleInvokeCommand(context);
                return;
            }

            // file upload
            if (Match($"{DefaultPath}/api/upload", HttpMethods.Post, request))
            {
                await HandleFileUpload(context);
                return;
            }

            _logger.LogInformation($"Received a request for path '{request.Path.ToString()}', but unable to find a handler for it.");

            await _next(context);
        }

        private bool IsPotentialMatch(HttpRequest request)
        {
            return request.Path.StartsWithSegments(_options.Path, StringComparison.InvariantCultureIgnoreCase) ||
                   request.Path.StartsWithSegments(DefaultPath, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool Match(string path, string method, HttpRequest request)
        {
            return request.Method == method &&
                   request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
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
                _behaviour.OnInitialize(httpContext, response);
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

        private async Task HandleInvokeCommand(HttpContext httpContext)
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
                _logger.LogError("An error occurred while processing the invoke command request.", e);
                await WriteErrorResponseAsync(e, httpContext);
            }
        }

        private async Task HandleFileUpload(HttpContext httpContext)
        {
            try
            {
                string body = await ReadBodyAsync(httpContext);
                WebCliResponse response = new WebCliResponse(httpContext);
                FileContent file = JsonConvert.DeserializeObject<FileContent>(body);
                FileId id = await _files.StoreAsync(file);
                response.WriteInformation("File upload completed, the file ID is:");
                response.WriteInformation(id.ToString());
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
