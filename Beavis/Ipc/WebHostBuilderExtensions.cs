using System;
using Beavis.Ipc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseBeavisServer(this IWebHostBuilder hostBuilder, Action<NamedPipeServerOptions> options)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.Configure(options);
                services.AddSingleton<NamedPipeServer, NamedPipeServer>();
                services.AddSingleton<IServer, BeavisServer>();
            });
        }
    }
}
