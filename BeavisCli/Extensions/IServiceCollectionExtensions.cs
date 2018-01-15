using System;
using BeavisCli;
using BeavisCli.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBeavisCli(this IServiceCollection services, Action<BeavisCliOptions> setupAction = null)
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
                    x.UnauthorizedApplicationExecutionAttemptHandler = new DefaultUnauthorizedApplicationExecutionAttemptHandler();
                    x.WelcomeHandler = new DefaultWelcomeHandler();
                };
            }

            var options = new BeavisCliOptions();
            setupAction(options);

            services.Configure(setupAction);

            services.AddSingleton<ApplicationProvider>();
            services.AddSingleton<ApplicationExecutor>();
            services.AddSingleton<WebRenderer>();



            


        
            return services;
        }

    }

    
}
