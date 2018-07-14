using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplicationDefinition(Name = "help", Description = "Displays help.")]
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
                    bool isDefault = _sandbox.IsDefault(application);

                    if (!_options.EnableDefaultApplicationsBrowsing && isDefault)
                    {
                        // ignore default applications if configured so
                        continue;
                    }

                    if (!application.IsBrowsable(context))
                    {
                        // ignore non-browsable applications
                        continue;
                    }

                    if (application.GetType() == GetType())
                    {
                        // ignore 'help'
                        continue;
                    }

                    if (isDefault)
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
                    lines.Add(new Tuple<string, string>(application.Meta().Name, application.Meta().Description));
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
