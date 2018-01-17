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
    internal class WebRenderer
    {
        public async Task RenderHtmlAsync(HttpResponse response)
        {
            var text = ReadFilesAsText("BeavisCli.Resources.html.index.html");

            await WriteAsync(text, response, "text/html");

        }

        public async Task RenderCssAsync(HttpResponse response)
        {
            var text = ReadFilesAsText("BeavisCli.Resources.css.jquery.terminal.min.css",
                                       "BeavisCli.Resources.css.site.css");

            await WriteAsync(text, response, "text/css");
        }

        public async Task RenderJsAsync(HttpResponse response)
        {
            var text = ReadFilesAsText("BeavisCli.Resources.js.jquery.min.js",
                                       "BeavisCli.Resources.js.jquery.terminal.min.js",
                                       "BeavisCli.Resources.js.jquery.mousewheel-min.js",
                                       "BeavisCli.Resources.js.angular.min.js",
                                       "BeavisCli.Resources.js.beavis-cli.js");

            await WriteAsync(text, response, "application/javascript");
        }

        public async Task RenderResponseAsync(WebCliResponse data, HttpResponse response)
        {
            if (data.Messages.Any())
            {
                data.WriteEmptyLine();
            }

            var text = JsonConvert.SerializeObject(data);

            await WriteAsync(text, response, "application/json");
        }

        private static async Task WriteAsync(string text, HttpResponse response, string contentType)
        {
            var content = Encoding.UTF8.GetBytes(text);
            response.ContentType = contentType;
            response.StatusCode = (int)HttpStatusCode.OK;
            await response.Body.WriteAsync(content, 0, content.Length);
        }

        private static string ReadFilesAsText(params string[] files)
        {
            var assembly = Assembly.GetAssembly(typeof(WebRenderer));

            var buf = new StringBuilder();

            foreach (var file in files)
            {
                using (var resourceStream = assembly.GetManifestResourceStream(file))
                {
                    using (var streamReader = new StreamReader(resourceStream))
                    {
                        var text = streamReader.ReadToEnd();
                        buf.AppendLine(text);
                    }
                }
            }

            return buf.ToString();
        }

    }
}
