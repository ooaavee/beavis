using System;
using BeavisCli;
using BeavisCli.Internal;
using BeavisCli.Internal.Applications;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBeavisCli(this IServiceCollection services, Action<WebCliOptions> setupAction = null)
        {
            if (setupAction == null)
            {
                setupAction = o =>
                {
                    o.EnableDefaultApplications = true;
                    o.EnableDefaultApplicationsBrowsing = true;
                    o.EnableFileUpload = true;
                    o.UnauthorizedHandler = new UnauthorizedHandler();
                    o.TerminalInitializer = new TerminalInitializer();
                    o.FileUploadStorage = new FileUploadStorage();
                };
            }

            var options = new WebCliOptions();
            setupAction(options);
            services.Configure(setupAction);

            services.AddSingleton<WebCliSandbox>();
            services.AddSingleton<WebRenderer>();
            services.AddSingleton<IJobPool, JobManager>();
            services.AddSingleton<JobManager>();

            if (options.EnableDefaultApplications)
            {
                services.AddSingletonWebCliApplication<Help>();
                services.AddSingletonWebCliApplication<Clear>();
                services.AddSingletonWebCliApplication<Reset>();
                services.AddSingletonWebCliApplication<Shortcuts>();

                if (options.EnableFileUpload)
                {
                    if (options.FileUploadStorage == null)
                    {
                        throw new InvalidOperationException("FileUpload is true, but FileUploadStorage has not been set.");
                    }
                    services.AddSingletonWebCliApplication<Upload>();
                }
            }


            services.AddMemoryCache();

           

            return services;
        }

        public static IServiceCollection AddScopedWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            return services.AddScoped<WebCliApplication, TWebCliApplication>();
        }

        public static IServiceCollection AddSingletonWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            return services.AddSingleton<WebCliApplication, TWebCliApplication>();
        }

        public static IServiceCollection AddTransientWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            return services.AddTransient<WebCliApplication, TWebCliApplication>();
        }
    }
}
