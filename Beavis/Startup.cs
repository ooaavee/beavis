using Beavis.Common.Ipc;
using Beavis.Configuration;
using Beavis.Host.Modules;
using Beavis.Ipc;
using Beavis.Isolation;
using Beavis.Isolation.Contracts;
using Beavis.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Beavis
{
    public class Startup
    {
        public class ForHost
        {
            private IConfiguration Configuration { get; }

            public ForHost(IConfiguration configuration)
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

                //var sss = app.ApplicationServices.GetService<IOptions<NamedPipeServerOptions>>();

                app.UseMiddleware<ModuleRequestProxy>();
            }
        }



        public class ForModule
        {
            public static ModuleStartupOptions Options { get; set; }

            private IConfiguration Configuration { get; }

            public ForModule(IConfiguration configuration)
            {
                Configuration = Options.GetConfiguration();
            }


            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton(new ConfigurationAccessor(Configuration));

                services.Configure<NamedPipeServerOptions>(opt =>
                {
                    opt.PipeName = Options.PipeName;
                });

                services.AddSingleton<IServer, IpcServer>();
                services.AddSingleton<NamedPipeServer, NamedPipeServer>();

            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
            }
        }



    }
}
