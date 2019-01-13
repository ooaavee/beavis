using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BeavisCli.Services
{
    public class DefaultCommandExecutionEnvironment : ICommandExecutionEnvironment
    {
        private readonly BeavisCliOptions _options;

        public DefaultCommandExecutionEnvironment(IOptions<BeavisCliOptions> options)
        {
            _options = options.Value;
        }

        public virtual bool IsAuthorized(CommandContext context)
        {
            // By default every command execution is authorized.
            // If you want some custom authorization logic, you should write your own
            // ICommandExecutionEnvironment implementation and register it during the startup.

            return true;
        }

        public virtual bool IsVisibleForHelp(ICommand cmd, HttpContext httpContext)
        {
            bool isBuiltIn = cmd.GetType().Assembly.Equals(typeof(ICommand).Assembly);

            if (isBuiltIn)
            {
                CommandInfo info = CommandInfo.Get(cmd);
                CommandDefinition definition = _options.BuiltInCommands[info.Name];
                if (!definition.IsVisibleForHelp)
                {
                    // ignore non-browsable commands
                    return false;
                }
            }

            return true;
        }

        public virtual bool IsTabCompletionEnabled(ICommand cmd, HttpContext httpContext)
        {
            bool isBuiltInCommand = cmd.GetType().Assembly.Equals(typeof(ICommand).Assembly);

            if (isBuiltInCommand)
            {
                CommandInfo info = CommandInfo.Get(cmd);
                CommandDefinition definition = _options.BuiltInCommands[info.Name];
                if (definition.IsEnabled && definition.IsTabCompletionEnabled)
                {
                    return true;
                }
            }

            return true;
        }
    }
}
