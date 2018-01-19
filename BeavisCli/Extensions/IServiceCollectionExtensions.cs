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
                setupAction = x =>
                {
                    x.UseDefaultApplications = true;
                    x.AreDefaultApplicationsBrowsable = true;
                    x.UnauthorizedHandler = new DefaultUnauthorizedHandler();
                    x.Greeter = new DefaultGreeter();
                };
            }

            var options = new WebCliOptions();
            setupAction(options);
            services.Configure(setupAction);

            services.AddSingleton<BeavisCliSandbox>();
            services.AddSingleton<WebRenderer>();

            if (options.UseDefaultApplications)
            {
                services.AddSingletonWebCliApplication<Help>();
                services.AddSingletonWebCliApplication<Clear>();
                services.AddSingletonWebCliApplication<Shortcuts>();
            }

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
