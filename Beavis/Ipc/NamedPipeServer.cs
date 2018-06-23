using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beavis.Ipc
{
    public class NamedPipeServer : IDisposable
    {
        private readonly NamedPipeServerOptions _options;
        private readonly List<NamedPipeServerInstance> _servers = new List<NamedPipeServerInstance>();

        public event EventHandler<PipeMessageEventArgs> OnRequest;

        public NamedPipeServer(NamedPipeServerOptions options)
        {
            _options = options;

            for (int i = 0; i < _options.InitialNumberOfServerInstances; i++)
            {
                NewServerInstance();
            }
        }

        public void Dispose()
        {
            CleanServers(true);
        }

        private void NewServerInstance()
        {
            // Start a new server instance only when the number of server instances is smaller than MaxNumberOfServerInstances
            if (_servers.Count < _options.MaxNumberOfServerInstances)
            {
                var server = new NamedPipeServerInstance(_options);

                server.OnConnected += (sender, e) =>
                {
                    NewServerInstance();
                };

                server.OnRequest += (sender, e) =>
                {
                    OnRequest?.Invoke(sender, e);
                };

                _servers.Add(server);
            }

            // Run clean servers anyway
            CleanServers(false);
        }

        /// <summary>
        /// A routine to clean NamedPipeServerInstances. When disposeAll is true,
        /// it will dispose all server instances. Otherwise, it will only dispose
        /// the instances that are completed, canceled, or faulted.
        /// PS: disposeAll is true only for this.Dispose()
        /// </summary>
        /// <param name="disposeAll"></param>
        private void CleanServers(bool disposeAll)
        {
            if (disposeAll)
            {
                foreach (var server in _servers)
                {
                    server.Dispose();
                }
            }
            else
            {
                for (int i = _servers.Count - 1; i >= 0; i--)
                {
                    if (_servers[i] == null)
                    {
                        _servers.RemoveAt(i);
                    }
                    else if (_servers[i].Communication != null)
                    {
                        TaskStatus status = _servers[i].Communication.Status;

                        if (status == TaskStatus.RanToCompletion || status == TaskStatus.Canceled || status == TaskStatus.Faulted)
                        {
                            _servers[i].Dispose();
                            _servers.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}
