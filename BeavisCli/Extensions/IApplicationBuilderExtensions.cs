using BeavisCli.Internal;
using BeavisCli.Internal.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebCli(this IApplicationBuilder app)
        {
            if (app.ApplicationServices.GetService(typeof(WebCliSandbox)) == null)
            {
                throw new InvalidOperationException(
                    $"Unable to find the required services. Please add all the required services by calling \'IServiceCollection.{nameof(IServiceCollectionExtensions.AddWebCli)}\' inside the call to \'ConfigureServices(...)\' in the application startup code.");
            }

            app.UseMiddleware<WebCliMiddleware>();

            return app;
        }
    }
}
