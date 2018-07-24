using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Demo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddWebCli(options =>
            {
                options.Path = "/beavis";
                options.DisplayExceptions = true;
            });

            //services.AddSingletonWebCliCommand<Services>();
            //services.AddSingletonWebCliCommand<Types>();


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseWebCli();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
