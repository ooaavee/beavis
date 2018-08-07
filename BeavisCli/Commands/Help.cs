using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("help", "Displays help.")]
    public class Help : Command
    {
        public override async Task ExecuteAsync(CommandContext context)
        {
            await OnExecuteAsync(() =>
            {
                // required services
                ICommandProvider commands = context.HttpContext.RequestServices.GetRequiredService<ICommandProvider>();
                ICommandExecutionEnvironment environment = context.HttpContext.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

                var defaults = new List<Command>();

                var externals = new List<Command>();

                foreach (Command cmd in commands.GetCommands(context.HttpContext))
                {
                    if (environment.IsVisibleForHelp(cmd, context))
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

                List<CommandInfo> items = defaults.Concat(externals).Select(cmd => cmd.Info).ToList();

                context.Response.WriteInformation("Default commands:");

                int lineCount = 0;

                string[] lines = ResponseRenderer.AsLines(items, x => x.Name, x => x.FullName, true);

                foreach (string line in lines)
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
