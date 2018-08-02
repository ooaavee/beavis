using BeavisCli.Commands;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;

namespace BeavisCli.Services
{
    public class TerminalBehaviour : ITerminalBehaviour
    {
        public virtual bool IsRequestHandlerAccessible(HttpContext httpContext, BeavisCliRequestTypes type)
        {
            return true;
        }

        public virtual IEnumerable<ResponseMessage> EnumInitMessages(HttpContext httpContext)
        {        
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            yield return new SuccessMessage($"{product.Product} {name.Version.Major}.{name.Version.Minor}.{name.Version.Build}");
            yield return new SuccessMessage($"{copyright.Copyright}. Code released under the MIT License. Usage 'license' for more details.");
        }

        public virtual IEnumerable<IJavaScriptStatement> EnumInitStatements(HttpContext httpContext)
        {
            yield return new SetTabCompletionCommands(EnumTabCompletionCommands(httpContext));
            yield return new SetUploadEnabled(IsUploadEnabled(httpContext));
        }

        public virtual bool IsVisibleForHelp(Command cmd, CommandContext context)
        {
            BeavisCliOptions options = context.HttpContext.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>().Value;

            if (cmd.GetType() == typeof(Help))
            {
                // ignore 'help'
                return false;
            }

            if (cmd.IsBuiltIn)
            {
                CommandDefinition definition = options.BuiltInCommands[cmd.Info.Name];
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

        public virtual bool IsUploadEnabled(HttpContext httpContext)
        {
            BeavisCliOptions options = httpContext.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>().Value;
            CommandInfo info = CommandInfo.ForType(typeof(Upload));
            CommandDefinition definition = options.BuiltInCommands[info.Name];
            return definition.IsEnabled;
        }

        protected virtual IEnumerable<string> EnumTabCompletionCommands(HttpContext context)
        {
            ICommandProvider commands = context.RequestServices.GetRequiredService<ICommandProvider>();

            foreach (Command cmd in commands.GetCommands(context))
            {
                if (IsTabCompletionEnabled(cmd, context))
                {
                    yield return cmd.Info.Name;
                }
            }
        }

        protected virtual bool IsTabCompletionEnabled(Command cmd, HttpContext context)
        {
            BeavisCliOptions options = context.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>().Value;

            bool enabled = false;

            if (cmd.IsBuiltIn)
            {
                CommandDefinition definition = options.BuiltInCommands[cmd.Info.Name];

                if (definition.IsEnabled && definition.IsTabCompletionEnabled)
                {
                    enabled = true;
                }
            }
            else
            {
                if (cmd.IsTabCompletionEnabled())
                {
                    enabled = true;
                }
            }

            return enabled;
        }
    }
}
