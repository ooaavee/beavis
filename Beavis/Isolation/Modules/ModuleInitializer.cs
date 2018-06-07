using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beavis.Isolation.Contracts;
using Beavis.Middlewares;
using Beavis.Modules;
using JKang.IpcServiceFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beavis.Isolation.Modules
{
    public class ModuleInitializer
    {
        private readonly ModuleRuntimeContract _contract;

        private ModuleInitializer(ModuleRuntimeContract contract)
        {
            _contract = contract;

          //  IConfiguration configuration

     
        }


        public static ServiceProvider Initialize(ModuleRuntimeContract contract)
        {
            IServiceCollection services = new ServiceCollection();
            ModuleInitializer initializer = new ModuleInitializer(contract);
            initializer.ConfigureServices(services);
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        /// <summary>
        /// Configure services
        /// </summary>
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddIpc(options => { options.ThreadCount = _contract.ThreadCount; });

            // must be transient!
            services.AddTransient<IIsolatedModule, IsolatedModule>();

            // must be singleton!
            var provider = new ModuleRequestHandlerProvider();
            services.AddSingleton<ModuleRequestHandlerProvider>(sp => provider);
        }

    }
}
