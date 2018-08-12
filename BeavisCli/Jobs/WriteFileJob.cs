using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeavisCli.Jobs
{
    public class WriteFileJob : IJob
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

        public Task RunAsync(HttpContext context, Response response)
        {
            IJavaScriptStatement js = new DownloadJs(_data, _fileName, _mimeType);
            string code = js.GetCode();
            response.Statements.Add(code);
            return Task.CompletedTask;
        }
    }
}
