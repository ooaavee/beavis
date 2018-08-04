using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("help", "Displays help.")]
    public class Help : Command
    {
        private readonly ICommandProvider _commands;
        private readonly IAuthorizationHandler _authorization;

        public Help(ICommandProvider commands, IAuthorizationHandler authorization)
        {
            _commands = commands;
            _authorization = authorization;
        }

        public override async Task ExecuteAsync(CommandContext context)
        {
            await OnExecuteAsync(() =>
            {
                var defaults = new List<Command>();

                var externals = new List<Command>();

                foreach (Command cmd in _commands.GetCommands(context.HttpContext))
                {
                    if (_authorization.IsVisibleForHelp(cmd, context))
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

                foreach (Command cmd in defaults.Concat(externals))
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
