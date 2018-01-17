using System;
using BeavisCli;
using BeavisCli.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBeavisCli(this IServiceCollection services, Action<WebCliOptions> setupAction = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                setupAction = x =>
                {
                    x.UseDefaultApplications = true;
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
                // TODO: register default apps here!
            }
                   
            return services;
        }
    }    
}
