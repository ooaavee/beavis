using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace Beavis.Ipc
{
    public class BeavisServer : IServer
    {
        private readonly NamedPipeServer _server;
        private readonly BeavisServerOptions _options;

        public IFeatureCollection Features { get; } = new FeatureCollection();

        public BeavisServer(NamedPipeServer server, IOptions<BeavisServerOptions> options)
        {
            _server = server;
            _options = options.Value;

            SetupFeatures();
        }

        private void SetupFeatures()
        {
            var serverAddressesFeature = new ServerAddressesFeature();
            serverAddressesFeature.Addresses.Add(_options.PipeName);

            Features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            Features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            Features.Set<IServerAddressesFeature>(serverAddressesFeature);
        }

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            _server.OnRequest += async delegate (object sender, PipeMessageEventArgs e)
            {
                e.Response = await HandleRequestAsync(application, e.Request);
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        private async Task<string> HandleRequestAsync<TContext>(IHttpApplication<TContext> application, string request)
        {
            BeavisHttpContext httpContext = null;

            bool processingStarted = false;

            try
            {
                try
                {
                    HostingApplication.Context context =
                        (HostingApplication.Context) (object) application.CreateContext(Features);

                    httpContext = new BeavisHttpContext(BeavisProtocol.CreateRequestModel(request));
                    context.HttpContext = httpContext;

                    processingStarted = true;

                    await application.ProcessRequestAsync((TContext)(object)context);
                }
                catch (Exception e)
                {
                    // TODO: Logging

                    if (processingStarted)
                    {
                        await httpContext.BeavisResponse.OnPipelineExceptionAsync(e, _options.ReturnStackTrace);
                    }
                }

                string response;

                if (processingStarted)
                {
                    response = BeavisProtocol.CreateResponseMessage(
                        httpContext.BeavisResponse,
                        BeavisProtocolResponseStatus.Succeed);
                }
                else
                {
                    response = BeavisProtocol.CreateResponseMessage(
                        null,
                        BeavisProtocolResponseStatus.Failed);
                }

                return response;
            }
            finally
            {
                httpContext?.Dispose();
            }
        }
    }
}
