using System;
using Beavis.Isolation;
using Beavis.Isolation.Contracts;
using Beavis.Isolation.Modules;
using Beavis.Middlewares;
using Beavis.Modules;
using JKang.IpcServiceFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beavis
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly ModuleStartupContract _contract;

        private bool IsHost => _contract == null;
        private bool IsModule => _contract != null;

        /// <summary>
        /// Constructor for 'host' mode.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Constructor for 'module' mode.
        /// </summary>
        public Startup(ModuleStartupContract contract)
        {
            _contract = contract;
        }
       
        /// <summary>
        /// Configure services, both host and modules.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            if (IsHost)
            {
                services.AddSingleton<ModuleManager, ModuleManager>();
                services.AddSingleton<IsolationManager, IsolationManager>();
                services.AddSingleton<IsolatedModuleProxy, IsolatedModuleProxy>();
            }

            if (IsModule)
            {
                services.AddIpc(options => { options.ThreadCount = _contract.ThreadCount; });

                // must be transient!
                services.AddTransient<IIsolatedModule, IsolatedModule>();

                // must be singleton!
                var provider = new ModuleRequestHandlerProvider();
                services.AddSingleton<ModuleRequestHandlerProvider>(sp => provider);
            }
        }

        /// <summary>
        /// Configure HTTP pipeline, host only.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (IsHost)
            {
                app.UseMiddleware<HttpRequestDispatcher>();

                app.Run(async (context) =>
                {

                    string key = context.Request.Path.ToString();
                    if (string.IsNullOrEmpty(key) || key.Length < 2)
                    {

                        await context.Response.WriteAsync("EMPTY");
                        return;
                    }

                    var mm = context.RequestServices.GetRequiredService<ModuleManager>();
                    var im = context.RequestServices.GetRequiredService<IsolationManager>();
                    var proxy = context.RequestServices.GetRequiredService<IsolatedModuleProxy>();

                    var module = mm.GetModuleByPath(key);
                    var handle = im.GetIsolatedModuleHandle(module);

                    string message = null;

                    if (module.HitCount > 0)
                    {
                        var envolope = await proxy.HandleRequestAsync(handle, context);

                        if (envolope.Succeed)
                        {
                            message = envolope.Content.Data;
                        }
                        else
                        {
                            message = envolope.Exception.ToString();
                        }
                    }
                    else
                    {
                        message = "Käynnistetty";
                    }

                    module.HitCount++;

                    await context.Response.WriteAsync(message);

                });
            }
        }




     

    }
}
