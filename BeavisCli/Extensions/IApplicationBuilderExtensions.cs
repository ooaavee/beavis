using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BeavisCli.Internal;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeavisCli(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (app.ApplicationServices.GetService(typeof(BeavisCliSandbox)) == null)
            {
                throw new InvalidOperationException(
                    $"Unable to find the required services. Please add all the required services by calling \'IServiceCollection.{nameof(IServiceCollectionExtensions.AddBeavisCli)}\' inside the call to \'ConfigureServices(...)\' in the application startup code.");
            }

            app.UseMiddleware<BeavisCliMiddleware>();

            return app;
        }
    }
}
