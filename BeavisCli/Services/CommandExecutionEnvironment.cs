using BeavisCli.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BeavisCli.Services
{
    public class CommandExecutionEnvironment : ICommandExecutionEnvironment
    {
        private readonly BeavisCliOptions _options;

        public CommandExecutionEnvironment(IOptions<BeavisCliOptions> options)
        {
            _options = options.Value;
        }

        public virtual bool IsKnownRequestType(BeavisCliRequestTypes type, HttpContext httpContext)
        {
            return true;
        }

        public virtual bool IsAuthorized(Command cmd, CommandContext context)
        {
            // By default every command execution is authorized. If you want some custom 
            // authorization logic, you should
            // - write your own ICommandExecutionEnvironment implementation and register it during the startup or/and
            // - override Command.IsAuthorized method

            return true;
        }

        public virtual bool IsVisibleForHelp(Command cmd, CommandContext context)
        {
            if (cmd.GetType() == typeof(Help))
            {
                // ignore 'help'
                return false;
            }

            if (cmd.IsBuiltIn)
            {
                CommandDefinition definition = _options.BuiltInCommands[cmd.Info.Name];
                if (!definition.IsVisibleForHelp)
                {
                    // ignore non-browsable commands
                    return false;
                }
            }

            if (!cmd.IsVisibleForHelp(context))
            {
                // ignore non-browsable commands
                return false;
            }

            return true;
        }

        public virtual bool IsTabCompletionEnabled(Command cmd, HttpContext httpContext)
        {
            if (cmd.IsBuiltIn)
            {
                CommandDefinition definition = _options.BuiltInCommands[cmd.Info.Name];

                if (definition.IsEnabled && definition.IsTabCompletionEnabled)
                {
                    return true;
                }
            }
            else
            {
                if (cmd.IsTabCompletionEnabled())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
