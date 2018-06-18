using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beavis.Ipc
{
    class NamedPipeServerInstance : IDisposable
    {
        private readonly NamedPipeServerStream _server;
        private bool _disposed;

        public Task Communication { get; private set; }

        public event EventHandler OnConnected = delegate { };
        public event EventHandler<PipeMsgEventArgs> newRequestEvent = delegate { };

        public NamedPipeServerInstance(string pipeName, int maxNumberOfServerInstances)
        {
            _server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, maxNumberOfServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
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

                OnConnected.Invoke(this, EventArgs.Empty);                

                Communication = Task.Factory.StartNew(OnCommunication);
            }
        }

        //private static int counter = 0;

        private void OnCommunication()
        {
            using (var reader = new StreamReader(_server))
            {
                while (!reader.EndOfStream)
                {
                    var request = reader.ReadLine();

                   // System.IO.File.WriteAllText($@"X:\work\logs\request_{Guid.NewGuid().ToString()}.txt", request);


                    if (request != null)
                    {
                        var msgEventArgs = new PipeMsgEventArgs(request);
                        newRequestEvent.Invoke(this, msgEventArgs);
                       var response = msgEventArgs.Response + Environment.NewLine;

                       
                        //System.IO.File.WriteAllText($@"C:\work\logs\response{Guid.NewGuid().ToString()}.txt", response);

                        var bytes = Encoding.UTF8.GetBytes(response);
                        _server.Write(bytes, 0, bytes.Count());
                    }
                }
            }
        }
    }
}
