using BeavisCli.Demo.Commands;
using BeavisCli.Demo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Demo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Demo")
                .AddCookie("Demo", options =>
                {

                });


            services.AddBeavisCli(options =>
            {
                options.Path = "/beavis";
                options.DisplayExceptions = true;
            });

            //services.AddSingletonCommand<Services>();
            //services.AddSingletonCommand<Types>();

            services.AddScopedCommand<Login>();


            services.AddScoped<UserService>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            app.UseBeavisCli();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
