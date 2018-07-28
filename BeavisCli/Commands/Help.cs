using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [WebCliCommand("help", "Displays help.")]
    public class Help : WebCliCommand
    {
        private readonly ICommandProvider _commands;
        private readonly ITerminalBehaviour _behaviour;

        public Help(ICommandProvider commands, ITerminalBehaviour behaviour)
        {
            _commands = commands;
            _behaviour = behaviour;
        }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                var defaults = new List<WebCliCommand>();

                var externals = new List<WebCliCommand>();

                foreach (WebCliCommand cmd in _commands.GetCommands(context.HttpContext))
                {
                    if (_behaviour.IsVisibleForHelp(cmd, context))
                    {
                        if (cmd.IsBuiltIn)
                        {
                            defaults.Add(cmd);
                        }
                        else
                        {
                            externals.Add(cmd);
                        }
                    }
                }
             
                var lines = new List<Tuple<string, string>>();

                foreach (WebCliCommand cmd in defaults.Concat(externals))
                {
                    lines.Add(new Tuple<string, string>(cmd.Info.Name, cmd.Info.FullName));
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
