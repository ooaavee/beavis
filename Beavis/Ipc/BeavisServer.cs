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

        public BeavisServer(NamedPipeServer server, IOptions<NamedPipeServerOptions> options)
        {
            _server = server;
            
            var serverAddressesFeature = new ServerAddressesFeature();
            serverAddressesFeature.Addresses.Add(options.Value.PipeName);

            Features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            Features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            Features.Set<IServerAddressesFeature>(serverAddressesFeature);
        }
       
        public IFeatureCollection Features { get; } = new FeatureCollection();

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            _server.OnRequest += async delegate (object sender, PipeMessageEventArgs e)
            {
                e.ResponseMessage = await HandleRequestAsync(application, e.RequestMessage);
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

        private async Task<string> HandleRequestAsync<TContext>(IHttpApplication<TContext> application, string requestMessage)
        {
            HostingApplication.Context context;
            BeavisHttpContext httpContext;
         
            try
            {
                context = (HostingApplication.Context)(object)application.CreateContext(Features);

                httpContext = CreateHttpContext(requestMessage);
                context.HttpContext = httpContext;
            }
            catch (Exception ex)
            {
                // TODO: Logging

                

                throw new NotImplementedException();
            }

            try
            {
                await application.ProcessRequestAsync((TContext)(object)context);
            }
            catch (Exception e)
            {
                // TODO: Lokita                
                // TODO: Palauta internal server error
                System.IO.File.WriteAllText(@"C:\work\virhe.txt", e.ToString());

                httpContext.BeavisResponse.OnErrr(e);
            }

            string responseMessage = BeavisProtocol.CreateResponseMessage(httpContext.BeavisResponse);

            return responseMessage;
        }



        private BeavisHttpContext CreateHttpContext(string requestMessage)
        {
            BeavisHttpContext httpContext = new BeavisHttpContext();
            httpContext.BeavisRequest = new BeavisHttpRequest(httpContext);
            httpContext.BeavisResponse = new BeavisHttpResponse(httpContext);

            return httpContext;
        }

     



       


    }
}
