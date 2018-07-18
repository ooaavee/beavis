using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplication("help", "Displays help.")]
    internal class Help : WebCliApplication
    {
        private readonly WebCliSandbox _sandbox;
        private readonly WebCliOptions _options;

        public Help(WebCliSandbox sandbox, IOptions<WebCliOptions> options)
        {
            _sandbox = sandbox;
            _options = options.Value;
        }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                var defaultApps = new List<WebCliApplication>();

                var externalApps = new List<WebCliApplication>();

                foreach (WebCliApplication application in _sandbox.GetApplications(context.HttpContext))
                {
                    bool builtIn = application.IsBuiltIn();

                    WebCliApplicationInfo info = application.GetInfo();

                    if (application.GetType() == GetType())
                    {
                        // ignore myself 
                        continue;
                    }

                    if (builtIn)
                    {
                        WebCliOptions.DefaultApplicationBehaviour behaviour = _options.DefaultApplications[info.Name];
                        if (!behaviour.IsVisibleForHelp)
                        {
                            // ignore non-browsable applications
                            continue;
                        }
                    }

                    if (!application.IsVisibleForHelp(context))
                    {
                        // ignore non-browsable applications
                        continue;
                    }

                    if (builtIn)
                    {
                        defaultApps.Add(application);
                    }
                    else
                    {
                        externalApps.Add(application);
                    }
                }
             
                var lines = new List<Tuple<string, string>>();

                foreach (WebCliApplication application in defaultApps.Concat(externalApps))
                {
                    WebCliApplicationInfo info = application.GetInfo();
                    lines.Add(new Tuple<string, string>(info.Name, info.Description));
                }

                context.Response.WriteInformation("Default applications:");

                int lineCount = 0;

                foreach (string line in ResponseRenderer.FormatLines(lines, true))
                {
                    lineCount++;
                    context.Response.WriteInformation(line);

                    if (externalApps.Any() && lineCount == defaultApps.Count)
                    {
                        context.Response.WriteEmptyLine();
                        context.Response.WriteInformation("Custom applications:");
                    }
                }
 
                return Exit(context);
            }, context);
        }
    }
}
