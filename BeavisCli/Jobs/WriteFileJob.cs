using BeavisCli.JavaScriptStatements;
using System.Threading.Tasks;

namespace BeavisCli.Jobs
{
    public class WriteFileJob : IJob
    {
        private readonly byte[] _content;
        private readonly string _fileName;
        private readonly string _mimeType;

        public WriteFileJob(byte[] content, string fileName, string mimeType)
        {
            _content = content;
            _fileName = fileName;
            _mimeType = mimeType;
        }

        public Task RunAsync(JobContext context)
        {
            IJavaScriptStatement js = new DownloadJs(_content, _fileName, _mimeType);
            string code = js.GetCode();
            context.Response.Statements.Add(code);
            return Task.CompletedTask;
        }
    }
}
