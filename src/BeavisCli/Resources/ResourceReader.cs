using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli.Resources
{
    internal static class ResourceReader
    {
        public static async Task<string> GetHtmlAsync()
        {
            return await GetResourcesAsync(
                "BeavisCli.Resources.html.index.html");
        }

        public static async Task<string> GetCssAsync()
        {
            return await GetResourcesAsync(
                "BeavisCli.Resources.css.jquery.terminal.min.css",
                "BeavisCli.Resources.css.site.css");
        }

        public static async Task<string> GetJsAsync()
        {
            return await GetResourcesAsync(
                "BeavisCli.Resources.js.jquery.min.js",
                "BeavisCli.Resources.js.jquery.terminal.min.js",
                "BeavisCli.Resources.js.jquery.mousewheel-min.js",
                "BeavisCli.Resources.js.angular.min.js",
                "BeavisCli.Resources.js.download.js",
                "BeavisCli.Resources.js.beaviscli.js");
        }

        public static async Task<string> GetLicenseAsync()
        {
            return await GetResourcesAsync(
                "BeavisCli.Resources.license.all.txt");
        }

        private static async Task<string> GetResourcesAsync(params string[] files)
        {
            var buf = new StringBuilder();

            foreach (var file in files)
            {
                using (var stream = Assembly.GetAssembly(typeof(ResourceReader)).GetManifestResourceStream(file))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string s = await reader.ReadToEndAsync();
                        buf.Append(s);
                    }
                }
            }

            return buf.ToString();
        }
    }
}
