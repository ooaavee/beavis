using Microsoft.AspNetCore.Http;

namespace Beavis.Ipc
{
    public static class HttpRequestExtensions
    {
        public static string GetModulePath(this HttpRequest request)
        {
            string path = request.Path.ToString();
            if (path.Length < 2)
            {
                return null;
            }
            int index = path.IndexOf('/', 1);
            return index < 0 ? path : path.Substring(0, index);
        }

        public static string GetSubPath(this HttpRequest request)
        {
            string path = request.Path.ToString();
            if (path.Length < 2)
            {
                return null;
            }
            int index = path.IndexOf('/', 1);
            return index < 0 ? "" : path.Substring(index);
        }
    }
}
