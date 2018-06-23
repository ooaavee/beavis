using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace Beavis.Ipc
{
    public class NamedPipeServerInstance : IDisposable
    {
        private readonly NamedPipeServerStream _server;
        private bool _disposed;

        public Task Communication { get; private set; }

        public event EventHandler OnConnected;
        public event EventHandler<PipeMessageEventArgs> OnRequest;

        public NamedPipeServerInstance(NamedPipeServerOptions options)
        {
            _server = new NamedPipeServerStream(options.PipeName,
                                                PipeDirection.InOut,
                                                options.MaxNumberOfServerInstances,
                                                PipeTransmissionMode.Message,
                                                PipeOptions.Asynchronous);

            IAsyncResult _ = _server.BeginWaitForConnection(ConnectionWaiter, null);
        }

        public void Dispose()
        {
            _disposed = true;
            _server.Dispose();
        }

        private void ConnectionWaiter(IAsyncResult result)
        {
            if (!_disposed)
            {
                _server.EndWaitForConnection(result);

                OnConnected?.Invoke(this, EventArgs.Empty);

                Communication = Task.Factory.StartNew(OnCommunication);
            }
        }

        private void OnCommunication()
        {
            using (var reader = new StreamReader(_server))
            {
                while (!reader.EndOfStream)
                {
                    // read request message
                    string request = reader.ReadLine();

                    // broadcast request message
                    PipeMessageEventArgs e = new PipeMessageEventArgs(request);
                    OnRequest?.Invoke(this, e);

                    // write response message
                    string response = e.Response + Environment.NewLine;
                    byte[] bytes = Encoding.UTF8.GetBytes(response);
                    _server.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
