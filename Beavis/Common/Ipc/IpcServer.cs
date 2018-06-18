using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace Beavis.Ipc
{
    public class IpcServer : IServer
    {
        private readonly NamedPipeServer _server;

        private int counter;

        public IpcServer(NamedPipeServer server)
        {
            _server = server;
            _server.OnRequestReceived += _server_OnRequestReceived;
        }

        private void _server_OnRequestReceived(object sender, PipeMsgEventArgs e)
        {
            e.Response = counter++.ToString();
        }

        public IFeatureCollection Features { get; } = new FeatureCollection();

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
