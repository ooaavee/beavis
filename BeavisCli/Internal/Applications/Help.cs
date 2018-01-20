using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BeavisCli.Internal.Applications
{
    internal class Help : WebCliApplication
    {
        private readonly BeavisCliSandbox _sandbox;
        private readonly WebCliOptions _options;

        public Help(BeavisCliSandbox sandbox, IOptions<WebCliOptions> options) : base("help", "Displays help")
        {
            _sandbox = sandbox;
            _options = options.Value;
        }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                var defaultApplications = new List<WebCliApplication>();
                var externalApplications = new List<WebCliApplication>();

                foreach (WebCliApplication app in _sandbox.GetApplications(context.HttpContext))
                {
                    if (!_options.AreDefaultApplicationsBrowsable)
                    {
                        if (IsDefault(app))
                        {
                            continue;
                        }
                    }

                    if (!app.IsBrowsable(context))
                    {
                        continue;
                    }

                    if (app.GetType() == GetType())
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(app.Name))
                    {
                        continue;
                    }

                    if (IsDefault(app))
                    {
                        defaultApplications.Add(app);
                    }
                    else
                    {
                        externalApplications.Add(app);
                    }
                }
             
                var allApplications = defaultApplications.Concat(externalApplications);

                var lines = new List<Tuple<string, string>>();
                foreach (WebCliApplication app in allApplications)
                {
                    lines.Add(new Tuple<string, string>(app.Name, app.Description));
                }

                context.Response.WriteInformation("List of supported applications:");

                foreach (string text in TerminalUtil.MakeBeautifulLines(lines))
                {
                    context.Response.WriteInformation(text);                    
                }

                context.Response.WriteEmptyLine();

                return Exit(context);
            }, context);
        }

        private bool IsDefault(WebCliApplication app)
        {
            return app.GetType().Assembly.Equals(GetType().Assembly);
        }

    }
}
