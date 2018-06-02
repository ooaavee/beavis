using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml;
using Beavis.Shared;
using JKang.IpcServiceFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beavis.Module
{
    public class Startup
    {
        private static StartupContract _contract;

        public static void Run(StartupContract contract)
        {
            _contract = contract;

            // build service provider
            IServiceCollection services = ConfigureServices(new ServiceCollection());
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            // TODO start IPC service host
            IpcServiceHostBuilder
                .Buid(contract.ModulePipeName, serviceProvider as IServiceProvider)
                .Run();

        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            //services
            //    .AddLogging(builder =>
            //    {
            //        builder.AddConsole();
            //        builder.SetMinimumLevel(LogLevel.Debug);
            //    });

            services
                .AddIpc(options =>
                {
                    options.ThreadCount = _contract.ModuleThreadCount;
                })
                .AddService<IModuleRequestHandler, ModuleRequestHandler>()
                ;

            return services;
        }

    }
}
