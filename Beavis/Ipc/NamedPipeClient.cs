using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Beavis.Ipc
{
    public class NamedPipeClient : IDisposable
    {
        public const int DefaultTimeOut = 1000;

        private readonly NamedPipeClientStream _client;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public NamedPipeClient(string pipeName) : this(pipeName, DefaultTimeOut) { }

        public NamedPipeClient(string pipeName, int timeOut)
        {
            _client = new NamedPipeClientStream(pipeName);
            _client.Connect(timeOut);
            _reader = new StreamReader(_client);
            _writer = new StreamWriter(_client) {AutoFlush = true};
        }

        public void Dispose()
        {
            _writer.Dispose();
            _reader.Dispose();
            _client.Dispose();
        }

        public async Task<string> SendRequestAsync(string request)
        {           
            await _writer.WriteLineAsync(request);
            var response = await _reader.ReadLineAsync();
            return response;
        }
    }
}
