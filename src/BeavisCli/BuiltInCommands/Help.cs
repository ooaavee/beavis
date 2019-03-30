using BeavisCli.Extensions;
using BeavisCli.Services;
using BeavisCli.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.BuiltInCommands
{
    [Command("help", "Displays help.")]
    [CommandBehaviour(false, true)]
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

                foreach (ICommand cmd in commands.All(context.HttpContext))
                {
                    if (!environment.IsVisibleForHelp(cmd, context.HttpContext))
                    {
                        continue;
                    }

                    if (IsBuiltInCommand(cmd))
                    {
                        builtInCount++;
                    }
                    else
                    {
                        externalCount++;
                    }

                    items.Add(cmd.GetType().GetCommandInfo());
                }

                string[] lines = LineFormatter.CreateLines(items, x => x.Name, x => x.Description, true, false);

                if (builtInCount > 0)
                {
                    context.WriteText("Built-in commands:");

                    for (int i = 0; i < builtInCount; i++)
                    {
                        string line = lines[i];
                        context.WriteText(line);
                    }
                }

                if (externalCount > 0)
                {
                    if (builtInCount > 0)
                    {
                        context.WriteText("");
                    }

                    context.WriteText("Custom commands:");

                    for (int i = builtInCount; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        context.WriteText(line);
                    }
                }
                
                if (!lines.Any())
                {
                    context.WriteError("No commands found.");
                }

                return context.Exit();
            });
        }

        private bool IsBuiltInCommand(ICommand cmd)
        {
            bool value = cmd.GetType().Assembly.Equals(typeof(ICommand).Assembly);
            return value;
        }
    }
}
