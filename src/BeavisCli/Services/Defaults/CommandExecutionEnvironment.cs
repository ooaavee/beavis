using BeavisCli.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BeavisCli.Services.Defaults
{
    public class CommandExecutionEnvironment : ICommandExecutionEnvironment
    {
        private readonly BeavisCliOptions _options;

        public CommandExecutionEnvironment(IOptions<BeavisCliOptions> options)
        {
            _options = options.Value;
        }

        public virtual async Task<bool> IsAuthorizedAsync(CommandContext context)
        {
            // Use AuthorizeCommandAttribute attributes to check if the command execution is authorized...
            IEnumerable<AuthorizeCommandAttribute> items =
                context.Command.GetType().GetCustomAttributes<AuthorizeCommandAttribute>();

            foreach (AuthorizeCommandAttribute item in items)
            {
                bool authorized = await item.IsAuthorizedAsync(context);
                if (!authorized)
                {
                    return false;
                }
            }

            // ...by default every command execution is authorized.
            return true;
        }

        public virtual bool IsVisibleForHelp(ICommand cmd, HttpContext context)
        {
            IEnumerable<CommandBehaviourAttribute> items = cmd.GetType().GetCustomAttributes<CommandBehaviourAttribute>();

            foreach (CommandBehaviourAttribute item in items)
            {
                if (!item.IsVisibleForHelp(cmd, context))
                {
                    return false;
                }
            }

            CommandInfo info = cmd.GetType().GetCommandInfo();

            if (_options.BuiltInCommandBehaviours.TryGetValue(info.Name, out CommandBehaviour behaviour))
            {
                if (!behaviour.IsEnabled || !behaviour.IsVisibleForHelp)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool IsTabCompletionEnabled(ICommand cmd, HttpContext context)
        {
            IEnumerable<CommandBehaviourAttribute> items = cmd.GetType().GetCustomAttributes<CommandBehaviourAttribute>();

            foreach (CommandBehaviourAttribute item in items)
            {
                if (!item.IsTabCompletionEnabled(cmd, context))
                {
                    return false;
                }
            }

            CommandInfo info = cmd.GetType().GetCommandInfo();

            if (_options.BuiltInCommandBehaviours.TryGetValue(info.Name, out CommandBehaviour behaviour))
            {
                if (!behaviour.IsEnabled || !behaviour.IsTabCompletionEnabled)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
