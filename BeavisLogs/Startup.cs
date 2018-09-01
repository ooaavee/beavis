using BeavisLogs.Commands.See;
using BeavisLogs.Drivers;
using BeavisLogs.Drivers.Serilog.AzureTableStorage;
using BeavisLogs.Services;
using BeavisLogs.Services.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BeavisLogs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithMachineName()
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // beavis cli
            services.AddBeavisCli(options =>
            {
                options.DisplayExceptions = true;
                options.Path = "/";
                options.Title = "Log viewer powered by Beavis CLI";
            });

            // our custom beavis cli commands
            services.AddTransientCommand<SeeCommand>();

            // application services
            services.AddTransient<IAccessProvider, AzureBlobStorageAccessProvider>();
            services.AddTransient<IDataSourceProvider, AzureBlobStorageDataSourceProvider>();

            // drivers
            services.AddTransient<IDriver, SerilogAzureTableStorageDriver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseBeavisCli();
        }
    }
}
