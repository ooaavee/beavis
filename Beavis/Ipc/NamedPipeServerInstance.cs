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

        public NamedPipeServerInstance(string pipeName, int maxNumberOfServerInstances)
        {
            _server = new NamedPipeServerStream(pipeName,
                                                PipeDirection.InOut,
                                                maxNumberOfServerInstances,
                                                PipeTransmissionMode.Message,
                                                PipeOptions.Asynchronous);

            var asyncResult = _server.BeginWaitForConnection(ConnectionWaiter, null);
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
                    string requestMessage = reader.ReadLine();

                    // broadcast request message
                    PipeMessageEventArgs e = new PipeMessageEventArgs(requestMessage);
                    OnRequest?.Invoke(this, e);

                    // write response message
                    byte[] bytes = Encoding.UTF8.GetBytes(e.ResponseMessage + Environment.NewLine);
                    _server.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
