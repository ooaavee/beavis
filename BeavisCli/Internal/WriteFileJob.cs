using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal
{
    internal class WriteFileJob : IJob
    {
        private readonly byte[] _data;
        private readonly string _fileName;
        private readonly string _mimeType;

        public WriteFileJob(byte[] data, string fileName, string mimeType)
        {
            _data = data;
            _fileName = fileName;
            _mimeType = mimeType;
        }

        public Task RunAsync(HttpContext context, WebCliResponse response)
        {
            IJavaScriptStatement js = new DownloadJs(_data, _fileName, _mimeType);
            response.AddJavaScript(js);
            return Task.CompletedTask;
        }
    }
}
