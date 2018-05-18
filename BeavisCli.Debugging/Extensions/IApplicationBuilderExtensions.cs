using BeavisCli.Internal;
using BeavisCli.Internal.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeavisCliDebugging(this IApplicationBuilder app)
        {
            app.UseBeavisCli();

            return app;
        }
    }
}
