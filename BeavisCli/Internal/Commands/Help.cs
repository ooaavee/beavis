using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Commands
{
    [WebCliCommand("help", "Displays help.")]
    internal class Help : WebCliCommand
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
                var defaults = new List<WebCliCommand>();

                var externals = new List<WebCliCommand>();

                foreach (WebCliCommand cmd in _sandbox.GetCommands(context.HttpContext))
                {
                    if (cmd.GetType() == GetType())
                    {
                        // ignore myself 
                        continue;
                    }

                    if (cmd.IsBuiltIn)
                    {
                        BuiltInCommandBehaviour behaviour = _options.BuiltInCommands[cmd.Info.Name];
                        if (!behaviour.IsVisibleForHelp)
                        {
                            // ignore non-browsable commands
                            continue;
                        }
                    }

                    if (!cmd.IsVisibleForHelp(context))
                    {
                        // ignore non-browsable commands
                        continue;
                    }

                    if (cmd.IsBuiltIn)
                    {
                        defaults.Add(cmd);
                    }
                    else
                    {
                        externals.Add(cmd);
                    }
                }
             
                var lines = new List<Tuple<string, string>>();

                foreach (WebCliCommand cmd in defaults.Concat(externals))
                {
                    lines.Add(new Tuple<string, string>(cmd.Info.Name, cmd.Info.Description));
                }

                context.Response.WriteInformation("Default commands:");

                int lineCount = 0;

                foreach (string line in ResponseRenderer.FormatLines(lines, true))
                {
                    lineCount++;
                    context.Response.WriteInformation(line);

                    if (externals.Any() && lineCount == defaults.Count)
                    {
                        context.Response.WriteEmptyLine();
                        context.Response.WriteInformation("Custom commands:");
                    }
                }
 
                return Exit(context);
            }, context);
        }
    }
}
