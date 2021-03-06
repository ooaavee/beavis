﻿using BeavisCli;
using DemoWebApp.Commands;
using DemoWebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DemoWebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Demo")
                .AddCookie("Demo");

            services.AddBeavisCli(options =>
            {
                //options.BuiltInCommandBehaviours["clear"].IsVisibleForHelp = false;
                //options.BuiltInCommandBehaviours["files"].IsVisibleForHelp = false;
                //options.BuiltInCommandBehaviours["help"].IsVisibleForHelp = false;
                //options.BuiltInCommandBehaviours["license"].IsVisibleForHelp = false;
                //options.BuiltInCommandBehaviours["reset"].IsVisibleForHelp = false;
                //options.BuiltInCommandBehaviours["shortcuts"].IsVisibleForHelp = false;
                //options.BuiltInCommandBehaviours["upload"].IsVisibleForHelp = false;

            });

            services.AddScoped<ICommand, Hello>();
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
