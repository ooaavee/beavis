using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisCli;
using BeavisLogs.Commands.See;
using BeavisLogs.Drivers;
using BeavisLogs.Drivers.Serilog.AzureTableStorage;
using BeavisLogs.Providers;
using BeavisLogs.Providers.Sqlite;
using BeavisLogs.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisLogs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScopedCommand<SeeCommand>();

            return services;
        }

        public static IServiceCollection AddDrivers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDriver, SerilogAzureTableStorageDriver>();

            return services;
        }


        public static IServiceCollection AddProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccessProvider, SqliteAccessProvider>();
            services.AddScoped<IDataSourceProvider, SqliteDataSourceProvider>();

            return services;
        }

        public static IServiceCollection AddTools(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<LogEventTempStorage>();
            services.AddScoped<LogEventMapper>();
            services.AddScoped<LogEventFormatter>();
            services.AddScoped<QueryBuilder>();

            services.AddSingleton(new ConfigurationAccessor(configuration));

            return services;
        }

    }
}
