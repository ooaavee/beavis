using BeavisCli.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class BeavisCliApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeavisCli(this IApplicationBuilder app)
        {
            if (!BeavisCliServiceCollectionExtensions.Flag)
            {
                throw new InvalidOperationException($"Unable to find the required services. Please add all the required services by calling \'IServiceCollection.{nameof(BeavisCliServiceCollectionExtensions.AddBeavisCli)}\' inside the call to \'ConfigureServices(...)\' in the application startup code.");
            }

            app.UseMiddleware<BeavisCliMiddleware>();

            return app;
        }
    }
}
