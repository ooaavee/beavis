using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddTransient<Generator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(Middleware);
        }

        private Task Middleware(HttpContext context, Func<Task> next)
        {
            Generator generator = context.RequestServices.GetRequiredService<Generator>();

            context.Response.WriteAsync("Hello World!!!");


            return Task.CompletedTask;

        }
    }
}
