using Beavis.Configuration;
using Beavis.Middlewares;
using Beavis.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beavis
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Startup_Host
    {
        private readonly IConfiguration _configuration;

        public Startup_Host(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {          
            services.AddSingleton(new ConfigurationAccessor(_configuration));
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
}
