using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BeavisCli.Internal
{
    internal static class WebCliRenderer
    {
        public static async Task RenderHtmlAsync(HttpContext httpContext)
        {
            string text = Read("BeavisCli.Resources.html.index.html");

            await WriteAsync(text, httpContext.Response, "text/html");
        }

        public static async Task RenderCssAsync(HttpContext httpContext)
        {
            string[] files = {
                "BeavisCli.Resources.css.jquery.terminal.min.css",
                "BeavisCli.Resources.css.site.css"
            };

            string text = Read(files);

            await WriteAsync(text, httpContext.Response, "text/css");
        }

        public static async Task RenderJsAsync(HttpContext httpContext)
        {
            string[] files = {
                "BeavisCli.Resources.js.jquery.min.js",
                "BeavisCli.Resources.js.jquery.terminal.min.js",
                "BeavisCli.Resources.js.jquery.mousewheel-min.js",
                "BeavisCli.Resources.js.angular.min.js",
                "BeavisCli.Resources.js.download.js",
                "BeavisCli.Resources.js.beavis-cli.js"
            };

            string text = Read(files);

            await WriteAsync(text, httpContext.Response, "application/javascript");
        }

        public static async Task RenderResponseAsync(WebCliResponse response, HttpContext httpContext)
        {
            if (response.Messages.Any())
            {
                response.WriteEmptyLine();
            }

            response.OnSending();

            string text = JsonConvert.SerializeObject(response);

            await WriteAsync(text, httpContext.Response, "application/json");
        }

        private static async Task WriteAsync(string text, HttpResponse response, string contentType)
        {
            byte[] content = Encoding.UTF8.GetBytes(text);
            response.ContentType = contentType;
            response.StatusCode = (int)HttpStatusCode.OK;
            await response.Body.WriteAsync(content, 0, content.Length);
        }

        private static string Read(params string[] files)
        {
            StringBuilder text = new StringBuilder();
            foreach (string file in files)
            {
                using (Stream stream = Assembly.GetAssembly(typeof(WebCliRenderer)).GetManifestResourceStream(file))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string tmp = reader.ReadToEnd();
                        text.AppendLine(tmp);
                    }
                }
            }
            return text.ToString();
        }
    }
}
