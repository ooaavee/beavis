using System;
using BeavisCli;
using BeavisCli.Debugging.Applications;
using BeavisCli.Internal;
using BeavisCli.Internal.Applications;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBeavisCliDebugging(this IServiceCollection services)
        {
            services.AddBeavisCli(o =>
            {
                o.Path = "/debug";
                o.EnableFileUpload = false;
            });

            services.AddSingletonWebCliApplication<Services>();
            services.AddSingletonWebCliApplication<Types>();

            return services;
        }       
    }
}
