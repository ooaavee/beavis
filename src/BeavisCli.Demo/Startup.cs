using BeavisCli.Demo.Commands;
using BeavisCli.Demo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Demo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Demo")
                .AddCookie("Demo");

            services.AddBeavisCli();

            services.AddScopedCommand<Login>();

            services.AddScopedCommand<Test>();

            services.AddScoped<IUserRepository, DemoUserRepository>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseBeavisCli();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
