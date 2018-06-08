using Beavis.Configuration;
using Beavis.Isolation;
using Beavis.Middlewares;
using Beavis.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beavis
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ModuleManager, ModuleManager>();
            services.AddSingleton<IsolationManager, IsolationManager>();
            services.AddSingleton<ConfigurationAccessor>(x => new ConfigurationAccessor(_configuration));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ModuleRequestProxy>();           
        }
    }
}
