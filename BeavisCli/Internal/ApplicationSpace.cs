using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Internal
{
    internal class ApplicationSpace
    {
        private ApplicationInfo[] _infos;

        public AbstractApplication FindApplicaton(CliContext context)
        {
            string name = context.Request.GetApplicationName();

            int matchCount = GetApplicationInfos(context).Count(x => x.Name == name);

            if (matchCount == 0)
            {
                throw new ApplicationSpaceException($"{name} is not a valid application.{Environment.NewLine}Usage 'help' to get list of applications.");
            }

            if (matchCount > 1)
            {
                throw new ApplicationSpaceException($"Found more than one application with name '{name}'. Application names must me unique.");
            }

            AbstractApplication app = GetApplications(context).First(x => x.Info.Name == name);
            return app;
        }

        public ApplicationInfo[] GetApplicationInfos(CliContext context)
        {
            return _infos ?? (_infos = GetApplications(context).Select(x => x.Info).ToArray());
        }

        private static IEnumerable<AbstractApplication> GetApplications(CliContext context)
        {
            IEnumerable<AbstractApplication> applications = context.HttpContext.RequestServices.GetServices<AbstractApplication>();
            return applications;
        }

    }
}