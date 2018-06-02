using System;
using System.Collections.Generic;
using System.Text;
using Beavis.Host.Isolation;
using Beavis.Host.Modules;
using Beavis.Shared;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beavis.Host
{
    public class Startup
    {
        public static void Run(StartupContract contract)
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ModuleManager, ModuleManager>();
            services.AddSingleton<IsolationManager, IsolationManager>();
            services.AddSingleton<IsolatedModuleProxy, IsolatedModuleProxy>();
        }

        private int counter = 0;

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            app.Run(async (context) =>
            {

                var mm = context.RequestServices.GetRequiredService<ModuleManager>();
                var im = context.RequestServices.GetRequiredService<IsolationManager>();
                var proxy = context.RequestServices.GetRequiredService<IsolatedModuleProxy>();

                var module = mm.GetModuleInfo("foobar");
                var handle = im.GetIsolatedModuleHandle(module);

                string message = null;
                if (counter > 1)
                {
                    try
                    {
                        var response = await proxy.XxxxAsync(handle, new ModuleRequest() {Data = "Töttöröö!"});
                        message = response.Data;
                    }
                    catch (Exception ex)
                    {
                        message = ex.ToString();
                    }
                }
                else
                {
                    message = "Käynnistetty";
                }

                await context.Response.WriteAsync(message);

                counter++;
            });
        }

    }
}
