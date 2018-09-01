using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BeavisLogs.TestDataGenerator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SerilogAzureTableStorageOptions>(Configuration);

            services.AddTransient<LogEventGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(Middleware);
        }

        private async Task Middleware(HttpContext context, Func<Task> next)
        {
            var service = context.RequestServices.GetRequiredService<LogEventGenerator>();
            await service.GenerateLogEventsAsync();
            await  context.Response.WriteAsync("done");
        }
    }
}
