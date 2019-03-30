using System;
using System.Threading.Tasks;
using BeavisCli.JsInterop.Statements;

namespace BeavisCli.Jobs.Defaults
{
    public class WriteFile : IJob
    {
        private readonly byte[] _content;
        private readonly string _fileName;
        private readonly string _mimeType;

        public WriteFile(byte[] content, string fileName, string mimeType)
        {
            _content = content ?? throw new ArgumentException(nameof(content));
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            _mimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
        }

        public Task RunAsync(JobContext context)
        {
            DownloadJs statement = new DownloadJs(_content, _fileName, _mimeType);
            context.Response.Statements.Add(statement.GetJs());
            return Task.CompletedTask;
        }
    }
}
