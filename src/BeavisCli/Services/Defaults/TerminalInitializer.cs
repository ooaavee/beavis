using BeavisCli.BuiltInCommands;
using BeavisCli.Extensions;
using BeavisCli.JsInterop;
using BeavisCli.JsInterop.Statements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;

namespace BeavisCli.Services.Defaults
{
    public class TerminalInitializer : ITerminalInitializer
    {
        private readonly BeavisCliOptions _options;

        public TerminalInitializer(IOptions<BeavisCliOptions> options)
        {
            _options = options.Value;
        }

        public virtual void Initialize(Response response, HttpContext context, bool silent = false)
        {
            if (!silent)
            {
                foreach (ResponseMessage message in GetMessages(context))
                {
                    response.Messages.Add(message);
                }
            }

            foreach (IStatement statement in GetStatements(context))
            {
                response.Statements.Add(statement.GetJs());
            }
        }

        protected virtual IEnumerable<ResponseMessage> GetMessages(HttpContext context)
        {        
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            yield return ResponseMessage.Success(product.Product);
            yield return ResponseMessage.Success($"{copyright.Copyright}. Code released under the MIT License. Usage 'license' for more details.");
        }

        protected virtual IEnumerable<IStatement> GetStatements(HttpContext context)
        {
            yield return new SetTabCompletionCommands(GetTabCompletionCommands(context));
            yield return new SetUploadEnabled(IsUploadEnabled(context));
            yield return new SetPrompt(_options.Prompt);
            yield return new SetTitle(_options.Title);
        }

        protected virtual IEnumerable<string> GetTabCompletionCommands(HttpContext context)
        {
            ICommandProvider commands = context.RequestServices.GetRequiredService<ICommandProvider>();
            ICommandExecutionEnvironment environment = context.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

            foreach (ICommand cmd in commands.All(context))
            {
                if (environment.IsTabCompletionEnabled(cmd, context))
                {
                    CommandInfo info = cmd.GetType().GetCommandInfo();
                    yield return info.Name;
                }
            }
        }

        protected virtual bool IsUploadEnabled(HttpContext context)
        {
            CommandInfo info = typeof(Upload).GetCommandInfo();
            CommandBehaviour behaviour = _options.BuiltInCommandBehaviours[info.Name];
            return behaviour.IsEnabled;
        }
    }
}
