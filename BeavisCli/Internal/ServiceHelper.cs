using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BeavisCli.Internal
{
    internal static class ServiceHelper
    {
        public static IEnumerable<WebCliCommand> GetWebCliCommands(this HttpContext httpContext)
        {
            return httpContext.RequestServices.GetServices<WebCliCommand>();
        }

        public static IUnauthorizedHandler GetUnauthorizedHandler(this WebCliContext context)
        {
            return context.HttpContext.RequestServices.GetRequiredService<IUnauthorizedHandler>();
        }

        public static JobPool GetJobPool(this HttpContext httpContext)
        {
            return httpContext.RequestServices.GetRequiredService<JobPool>();
        }

        public static ILoggerFactory GetLoggerFactory(this WebCliContext context)
        {
            return context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        }
    }
}
