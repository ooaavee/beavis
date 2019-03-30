using BeavisCli.Jobs;
using BeavisCli.Resources;
using BeavisCli.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Middlewares
{
    public class BeavisCliMiddleware
    {
        private const string ApiRoot = "/api/beaviscli";

        private readonly RequestDelegate _next;
        private readonly ILogger<BeavisCliMiddleware> _logger;
        private readonly BeavisCliOptions _options;

        public BeavisCliMiddleware(RequestDelegate next, ILogger<BeavisCliMiddleware> logger, IOptions<BeavisCliOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            BeavisCliRequestTypes type = GetRequestType(context.Request);

            if (!_options.IsRequestApproved(type, context))
            {
                await _next(context);
                return;
            }

            try
            {
                _logger.LogDebug($"Started to process a request for a path '{context.Request.Path.ToString()}'.");

                switch (type)
                {
                    case BeavisCliRequestTypes.GetTerminalHtml:
                        await GetTerminalHtmlAsync(context);
                        break;

                    case BeavisCliRequestTypes.GerTerminalCss:
                        await GetTerminalCssAsync(context);
                        break;

                    case BeavisCliRequestTypes.GerTerminalJs:
                        await GetTerminalJsAsync(context);
                        break;

                    case BeavisCliRequestTypes.InitializeTerminal:
                        await InitializeTerminalAsync(context);
                        break;

                    case BeavisCliRequestTypes.RunJob:
                        await RunJobAsync(context);
                        break;

                    case BeavisCliRequestTypes.HandleCommand:
                        await HandleCommandAsync(context);
                        break;

                    case BeavisCliRequestTypes.UploadFile:
                        await UploadFileAsync(context);
                        break;

                    case BeavisCliRequestTypes.None:
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while processing the request type of '{type}'.");
                await WriteErrorResponseAsync(e, context);
                return;
            }

            _logger.LogDebug($"Processed a request for a path '{context.Request.Path.ToString()}'.");
        }
       
        private static async Task GetTerminalHtmlAsync(HttpContext context)
        {
            string s = await ResourceReader.GetHtmlAsync();
            await WriteAsync(s, context.Response, "text/html");
        }

        private static async Task GetTerminalCssAsync(HttpContext context)
        {
            string s = await ResourceReader.GetCssAsync();
            await WriteAsync(s, context.Response, "text/css");
        }

        private static async Task GetTerminalJsAsync(HttpContext context)
        {
            string s = await ResourceReader.GetJsAsync();
            await WriteAsync(s, context.Response, "application/javascript");
        }

        private static async Task InitializeTerminalAsync(HttpContext context)
        {
            ITerminalInitializer initializer = context.RequestServices.GetRequiredService<ITerminalInitializer>();
            Response response = new Response();
            initializer.Initialize(response, context);
            await RenderResponseAsync(response, context);
        }

        private static async Task RunJobAsync(HttpContext context)
        {
            IJobPool jobs = context.RequestServices.GetRequiredService<IJobPool>();
            Response response = new Response();
            StringValues key = context.Request.Query["key"];
            string content = await ReadBodyAsync(context);
            JobContext jc = new JobContext(content, context, response);
            await jobs.RunAsync(key, jc);
            await RenderResponseAsync(response, context);
        }

        private static async Task HandleCommandAsync(HttpContext context)
        {
            IRequestHandler executor = context.RequestServices.GetRequiredService<IRequestHandler>();
            string json = await ReadBodyAsync(context);
            Request request = JsonConvert.DeserializeObject<Request>(json);
            Response response = await executor.HandleAsync(request, context);
            await RenderResponseAsync(response, context);
        }

        private static async Task UploadFileAsync(HttpContext context)
        {
            IFileStorage files = context.RequestServices.GetRequiredService<IFileStorage>();
            string body = await ReadBodyAsync(context);
            Response response = new Response();
            FileContent file = JsonConvert.DeserializeObject<FileContent>(body);
            if (string.IsNullOrEmpty(file.Type))
            {
                file.Type = FileContent.ResolveType(file.Name);
            }
            string id = await files.StoreAsync(file);
            response.Messages.Add(ResponseMessage.Plain("File upload completed, the file ID is:"));
            response.Messages.Add(ResponseMessage.Plain(id));
            await RenderResponseAsync(response, context);
        }

        private BeavisCliRequestTypes GetRequestType(HttpRequest request)
        {
            bool potentialMatch =
                request.Path.StartsWithSegments(_options.Path, StringComparison.InvariantCultureIgnoreCase) ||
                request.Path.StartsWithSegments(ApiRoot, StringComparison.InvariantCultureIgnoreCase);

            if (!potentialMatch)
            {
                return BeavisCliRequestTypes.None;
            }

            bool Match(string path, string method)
            {
                return request.Method == method && request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
            }

            if (Match(_options.Path, HttpMethods.Get))
            {
                return BeavisCliRequestTypes.GetTerminalHtml;
            }

            if (Match($"{ApiRoot}/css", HttpMethods.Get))
            {
                return BeavisCliRequestTypes.GerTerminalCss;
            }

            if (Match($"{ApiRoot}/js", HttpMethods.Get))
            {
                return BeavisCliRequestTypes.GerTerminalJs;
            }

            if (Match($"{ApiRoot}/initialize", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.InitializeTerminal;
            }

            if (Match($"{ApiRoot}/job", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.RunJob;
            }

            if (Match($"{ApiRoot}/request", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.HandleCommand;
            }

            if (Match($"{ApiRoot}/upload", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.UploadFile;
            }

            return BeavisCliRequestTypes.None;
        }

        private async Task WriteErrorResponseAsync(Exception e, HttpContext context)
        {
            string text = _options.DisplayExceptions
                ? e.ToString()
                : "An error occurred. Please check your application logs for more details.";

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(text, Encoding.UTF8);
        }

        private static async Task<string> ReadBodyAsync(HttpContext context)
        {         
            using (var stream = context.Request.Body)
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string s = await reader.ReadToEndAsync();
                    return s;
                }
            }
        }

        private static async Task RenderResponseAsync(Response response, HttpContext context)
        {
            if (response.RenderMode == ResponseRenderMode.Readable)
            {
                if (response.Messages.Any())
                {
                    response.Messages.Add(ResponseMessage.Plain(string.Empty));
                }
            }

            response.OnSending();

            string text = JsonConvert.SerializeObject(response);

            await WriteAsync(text, context.Response, "application/json");
        }

        private static async Task WriteAsync(string text, HttpResponse response, string contentType)
        {
            byte[] content = Encoding.UTF8.GetBytes(text);
            response.ContentType = contentType;
            response.StatusCode = (int) HttpStatusCode.OK;
            await response.Body.WriteAsync(content, 0, content.Length);
        }
    }
}
