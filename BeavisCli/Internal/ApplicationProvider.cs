using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeavisCli.Internal
{
    internal class ApplicationProvider
    {
        private readonly BeavisCliOptions _options;

        public ApplicationProvider(IOptions<BeavisCliOptions> options)
        {
            _options = options.Value;
        }

        public AbstractApplication FindApplicaton(string name, HttpContext httpContext)
        {
            int matchCount = 0;

            AbstractApplication result = null;

            foreach (AbstractApplication application in GetApplications(httpContext))
            {
                ApplicationInfo info = application.GetInfo();

                if (info.Name == name)
                {
                    matchCount++;
                    result = application;
                }

                if (matchCount > 1)
                {
                    throw new ApplicationProviderException($"Found more than one application with name '{name}'. Host names must me unique.");
                }
            }

            if (matchCount == 0)
            {
                if (_options.UseDefaultApplications)
                {
                    throw new ApplicationProviderException($"{name} is not a valid application.{Environment.NewLine}Usage 'help' to get list of applications.");
                }
                else
                {
                    throw new ApplicationProviderException($"{name} is not a valid application.");
                }
            }

            return result;
        }

        public IEnumerable<ApplicationInfo> GetApplicationInfos(HttpContext httpContext)
        {
            foreach (AbstractApplication application in GetApplications(httpContext))
            {
                yield return application.GetInfo();
            }
        }

        private static IEnumerable<AbstractApplication> GetApplications(HttpContext httpContext)
        {
            IEnumerable<AbstractApplication> applications = httpContext.RequestServices.GetServices<AbstractApplication>();
            return applications;
        }

    }
}