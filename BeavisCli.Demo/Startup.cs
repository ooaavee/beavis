using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Demo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddBeavisCli(options =>
            {
                options.Path = "/beavis";
                options.DisplayExceptions = true;
            });

            //services.AddSingletonCommand<Services>();
            //services.AddSingletonCommand<Types>();


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseBeavisCli();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
