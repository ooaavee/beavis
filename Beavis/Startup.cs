using System;
using Beavis.Configuration;
using Beavis.Ipc;
using Beavis.Middlewares;
using Beavis.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beavis
{
    public class Startup
    {
        public class Host
        {
            private IConfiguration Configuration { get; }

            public Host(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton(new ConfigurationAccessor(Configuration));
                services.AddSingleton<ModuleHostingEnvironment>();
                services.AddSingleton<ModuleManager>();
                services.AddSingleton<ModuleDeployer>();
                services.AddSingleton<ModuleRunner>();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseMiddleware<RequestProxyMiddleware>();
            }
        }



        public class Module
        {
            public static ModuleStartupOptions Options { get; set; }

            private IConfiguration Configuration { get; }

            public Module(IConfiguration configuration)
            {
                Configuration = Options.GetConfiguration();
            }


            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton(new ConfigurationAccessor(Configuration));
                services.AddSingleton<Laskuri>();

            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {

                app.Run(async context =>
                {
                    Laskuri laskuri = context.RequestServices.GetRequiredService<Laskuri>();

                    //throw new NotImplementedException("huuhaa!!!! testaan virheitä");

                    await context.Response.WriteAsync("Hello, World! " + laskuri.Value++);
                });
            }
        }


        public class Laskuri
        {
            public int Value { get; set; }
        }

    }
}
