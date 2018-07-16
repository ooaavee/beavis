using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeavisCli.Internal
{
    internal static class ServiceHelper
    {
        public static IEnumerable<WebCliApplication> GetWebCliApplications(this HttpContext httpContext)
        {
            return httpContext.RequestServices.GetServices<WebCliApplication>();
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
