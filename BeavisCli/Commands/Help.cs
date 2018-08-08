using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("help", "Displays help.")]
    public class Help : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {
                // required services
                ICommandProvider commands = context.HttpContext.RequestServices.GetRequiredService<ICommandProvider>();
                ICommandExecutionEnvironment environment = context.HttpContext.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

                var defaults = new List<ICommand>();
                var externals = new List<ICommand>();

                List<CommandInfo> items = new List<CommandInfo>();

                foreach (ICommand cmd in commands.GetCommands(context.HttpContext))
                {
                    if (environment.IsVisibleForHelp(cmd, context.HttpContext))
                    {
                        bool isBuiltInCommand = cmd.GetType().Assembly.Equals(typeof(ICommand).Assembly);

                        if (isBuiltInCommand)
                        {
                            defaults.Add(cmd);
                        }
                        else
                        {
                            externals.Add(cmd);
                        }

                        items.Add(CommandInfo.Get(cmd));
                    }
                }

                context.Response.WriteInformation("Default commands:");

                int lineCount = 0;

                string[] lines = LineFormatter.FormatLines(items, x => x.Name, x => x.FullName, true);

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
 
                return context.Exit();
            });
        }
    }
}
