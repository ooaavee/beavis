using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Beavis.Ipc
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseBeavisServer(this IWebHostBuilder hostBuilder, Action<BeavisServerOptions> setup)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                // Options for the BeavisServer
                var options = new BeavisServerOptions();
                setup(options);
                services.Configure(setup);

                // BeavisServer
                services.AddSingleton<IServer, BeavisServer>();

                // This is the named pipes server service used by the BeavisServer
                services.AddSingleton(new NamedPipeServer(new NamedPipeServerOptions
                {
                    PipeName = options.PipeName
                }));
            });
        }
    }
}
