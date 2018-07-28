using BeavisCli.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebCli(this IApplicationBuilder app)
        {
            if (!IServiceCollectionExtensions.ConfigureServicesFlag)
            {
                throw new InvalidOperationException($"Unable to find the required services. Please add all the required services by calling \'IServiceCollection.{nameof(IServiceCollectionExtensions.AddWebCli)}\' inside the call to \'ConfigureServices(...)\' in the application startup code.");
            }

            app.UseMiddleware<WebCliMiddleware>();

            return app;
        }
    }
}
