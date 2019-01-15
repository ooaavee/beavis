using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("help", "Displays help.")]
    public class Help : ICommand
    {
        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {
                // required services
                ICommandProvider commands = context.HttpContext.RequestServices.GetRequiredService<ICommandProvider>();
                ICommandExecutionEnvironment environment = context.HttpContext.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

                int builtInCount = 0;
                int externalCount = 0;

                var items = new List<CommandInfo>();

                foreach (ICommand cmd in commands.GetCommands(context.HttpContext))
                {
                    if (cmd == this)
                    {
                        continue;
                    }

                    if (!environment.IsVisibleForHelp(cmd, context.HttpContext))
                    {
                        continue;
                    }

                    bool isBuiltInCommand = cmd.GetType().Assembly.Equals(typeof(ICommand).Assembly);

                    if (isBuiltInCommand)
                    {
                        builtInCount++;
                    }
                    else
                    {
                        externalCount++;
                    }

                    items.Add(CommandInfo.Get(cmd));
                }

                context.WriteText("Built-in commands:");

                int lineCount = 0;

                string[] lines = LineFormatter.CreateLines(items, x => x.Name, x => x.FullName, true, false);

                foreach (string line in lines)
                {
                    lineCount++;
                    context.WriteText(line);

                    if (externalCount > 0 && lineCount == builtInCount)
                    {
                        context.WriteText("");
                        context.WriteText("Custom commands:");
                    }
                }
 
                return context.Exit();
            });
        }
    }
}
