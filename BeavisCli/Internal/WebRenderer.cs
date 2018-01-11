using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal
{
    internal class WebRenderer
    {
        private static string _htmlFile = "BeavisCli.Resources.html.index.html";

        private static string[] _jsFiles = {
            "BeavisCli.Resources.js.jquery.min.js",
            "BeavisCli.Resources.js.jquery.terminal.min.js",
            "BeavisCli.Resources.js.jquery.mousewheel-min.js",
            "BeavisCli.Resources.js.angular.min.js",
            "BeavisCli.Resources.js.beavis-cli.js"
        };

        private static string[] _cssFiles = {
            "BeavisCli.Resources.css.jquery.terminal.min.css",
            "BeavisCli.Resources.css.site.css"
        };

        public async Task RenderHtmlAsync(HttpResponse response)
        {
            string s = ReadFiles(_htmlFile);
            byte[] content = Encoding.UTF8.GetBytes(s);

            response.ContentType = "text/html";
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.Body.WriteAsync(content, 0, content.Length);
        }

        public async Task RenderCssAsync(HttpResponse response)
        {
            string s = ReadFiles(_cssFiles);
            byte[] content = Encoding.UTF8.GetBytes(s);

            response.ContentType = "text/css";
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.Body.WriteAsync(content, 0, content.Length);
        }

        public async Task RenderJsAsync(HttpResponse response)
        {
            string s = ReadFiles(_jsFiles);
            byte[] content = Encoding.UTF8.GetBytes(s);

            response.ContentType = "application/javascript";
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.Body.WriteAsync(content, 0, content.Length);
        }

        private static string ReadFiles(params string[] files)
        {
            StringBuilder buf = new StringBuilder();
            foreach (string file in files)
            {
                string s = ReadEmbededResource(file);
                buf.AppendLine(s);
            }
            return buf.ToString();
        }

        public static string ReadEmbededResource(string name)
        {
            var assembly = Assembly.GetAssembly(typeof(WebRenderer));
            using (var resourceStream = assembly.GetManifestResourceStream(name))
            {
                using (var streamReader = new StreamReader(resourceStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

    }
}
