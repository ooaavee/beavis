using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly RequestDelegate _next;
        private readonly ILogger<BeavisCliMiddleware> _logger;
        private readonly BeavisCliOptions _options;

        public BeavisCliMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<BeavisCliOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<BeavisCliMiddleware>();
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            BeavisCliRequestTypes type = GetRequestType(context.Request);

            if (!IsEnabled(type, context))
            {
                await _next(context);
                return;
            }

            try
            {
                _logger.LogDebug($"Started to process a request for a path '{context.Request.Path.ToString()}'.");

                switch (type)
                {
                    case BeavisCliRequestTypes.Html:
                        await HandleHtml(context);
                        break;

                    case BeavisCliRequestTypes.Css:
                        await HandleCss(context);
                        break;

                    case BeavisCliRequestTypes.Js:
                        await HandleJs(context);
                        break;

                    case BeavisCliRequestTypes.Initialize:
                        await HandleInitialize(context);
                        break;

                    case BeavisCliRequestTypes.Job:
                        await HandleJob(context);
                        break;

                    case BeavisCliRequestTypes.Command:
                        await HandleCommand(context);
                        break;

                    case BeavisCliRequestTypes.Upload:
                        await HandleUpload(context);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while processing the request type of '{type}'.", e);
                await WriteErrorResponseAsync(e, context);
                return;
            }

            _logger.LogDebug($"Processed a request for a path '{context.Request.Path.ToString()}'.");
        }

        private bool IsEnabled(BeavisCliRequestTypes type, HttpContext context)
        {
            if (type == BeavisCliRequestTypes.None)
            {
                return false;
            }

            Func<BeavisCliRequestTypes, HttpContext, bool> checker = _options.IsRequestTypeBlocked;

            if (checker != null && checker(type, context))
            {
                return false;
            }

            return true;
        }

        private static async Task HandleHtml(HttpContext context)
        {
            var text = await GetResourcesAsync("BeavisCli.Resources.html.index.html");

            await WriteAsync(text, context.Response, "text/html");
        }

        private static async Task HandleCss(HttpContext context)
        {
            var text = await GetResourcesAsync(
                "BeavisCli.Resources.css.jquery.terminal.min.css",
                "BeavisCli.Resources.css.site.css");

            await WriteAsync(text, context.Response, "text/css");
        }

        private static async Task HandleJs(HttpContext context)
        {
            var text = await GetResourcesAsync(
                "BeavisCli.Resources.js.jquery.min.js",
                "BeavisCli.Resources.js.jquery.terminal.min.js",
                "BeavisCli.Resources.js.jquery.mousewheel-min.js",
                "BeavisCli.Resources.js.angular.min.js",
                "BeavisCli.Resources.js.download.js",
                "BeavisCli.Resources.js.beaviscli.js");

            await WriteAsync(text, context.Response, "application/javascript");
        }

        private static async Task HandleInitialize(HttpContext context)
        {
            var initializer = context.RequestServices.GetRequiredService<ITerminalInitializer>();
            var response = new Response();
            initializer.Initialize(response, context);
            await RenderResponseAsync(response, context);
        }

        private static async Task HandleJob(HttpContext context)
        {
            var jobs = context.RequestServices.GetRequiredService<IJobPool>();
            var response = new Response();
            var key = context.Request.Query["key"];
            var content = await ReadBodyAsync(context);
            var jc = new JobContext(content, context, response);
            await jobs.RunAsync(key, jc);
            await RenderResponseAsync(response, context);
        }

        private static async Task HandleCommand(HttpContext context)
        {
            var executor = context.RequestServices.GetRequiredService<IRequestHandler>();
            var json = await ReadBodyAsync(context);
            var request = JsonConvert.DeserializeObject<Request>(json);
            var response = await executor.HandleAsync(request, context);
            await RenderResponseAsync(response, context);
        }

        private static async Task HandleUpload(HttpContext context)
        {
            var files = context.RequestServices.GetRequiredService<IFileStorage>();
            var body = await ReadBodyAsync(context);
            var response = new Response();
            var file = JsonConvert.DeserializeObject<FileContent>(body);
            if (string.IsNullOrEmpty(file.Type))
            {
                file.Type = FileContent.ResolveType(file.Name);
            }
            var id = await files.StoreAsync(file);
            response.Messages.Add(ResponseMessage.Plain("File upload completed, the file ID is:"));
            response.Messages.Add(ResponseMessage.Plain(id));
            await RenderResponseAsync(response, context);
        }

        private BeavisCliRequestTypes GetRequestType(HttpRequest request)
        {
            const string fixedPath = "/beaviscli-api";

            bool IsPotentialMatch()
            {
                return request.Path.StartsWithSegments(_options.Path, StringComparison.InvariantCultureIgnoreCase) ||
                       request.Path.StartsWithSegments(fixedPath, StringComparison.InvariantCultureIgnoreCase);
            }

            if (!IsPotentialMatch())
            {
                return BeavisCliRequestTypes.None;
            }

            bool Match(string path, string method)
            {
                return request.Method == method && request.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
            }

            if (Match(_options.Path, HttpMethods.Get))
            {
                return BeavisCliRequestTypes.Html;
            }

            if (Match($"{fixedPath}/css", HttpMethods.Get))
            {
                return BeavisCliRequestTypes.Css;
            }

            if (Match($"{fixedPath}/js", HttpMethods.Get))
            {
                return BeavisCliRequestTypes.Js;
            }

            if (Match($"{fixedPath}/initialize", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.Initialize;
            }

            if (Match($"{fixedPath}/job", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.Job;
            }

            if (Match($"{fixedPath}/request", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.Command;
            }

            if (Match($"{fixedPath}/upload", HttpMethods.Post))
            {
                return BeavisCliRequestTypes.Upload;
            }

            return BeavisCliRequestTypes.None;
        }

        private async Task WriteErrorResponseAsync(Exception e, HttpContext context)
        {
            var text = _options.DisplayExceptions ?
                e.ToString() :
                "An error occurred. Please check your application logs for more details.";

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
                    var s = await reader.ReadToEndAsync();
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

            var text = JsonConvert.SerializeObject(response);

            await WriteAsync(text, context.Response, "application/json");
        }

        private static async Task WriteAsync(string text, HttpResponse response, string contentType)
        {
            var content = Encoding.UTF8.GetBytes(text);
            response.ContentType = contentType;
            response.StatusCode = (int)HttpStatusCode.OK;
            await response.Body.WriteAsync(content, 0, content.Length);
        }

        private static async Task<string> GetResourcesAsync(params string[] files)
        {
            var buf = new StringBuilder();

            foreach (var file in files)
            {
                using (var stream = Assembly.GetAssembly(typeof(BeavisCliMiddleware)).GetManifestResourceStream(file))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var s = await reader.ReadToEndAsync();
                        buf.Append(s);
                    }
                }
            }

            return buf.ToString();
        }
    }
}
