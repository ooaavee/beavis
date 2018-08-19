using BeavisCli.Commands;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;

namespace BeavisCli.Services
{
    public class TerminalInitializer : ITerminalInitializer
    {
        private readonly BeavisCliOptions _options;

        public TerminalInitializer(IOptions<BeavisCliOptions> options)
        {
            _options = options.Value;
        }

        public virtual void Initialize(Response response, HttpContext httpContext, bool silent = false)
        {
            if (!silent)
            {
                foreach (ResponseMessage message in GetMessages(httpContext))
                {
                    response.Messages.Add(message);
                }
            }

            foreach (IJavaScriptStatement js in GetJavaScript(httpContext))
            {
                string code = js.GetCode();
                response.Statements.Add(code);
            }
        }

        protected virtual IEnumerable<ResponseMessage> GetMessages(HttpContext httpContext)
        {        
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            yield return ResponseMessage.Success($"{product.Product} {name.Version.Major}.{name.Version.Minor}.{name.Version.Build}");
            yield return ResponseMessage.Success($"{copyright.Copyright}. Code released under the MIT License. Usage 'license' for more details.");
        }

        protected virtual IEnumerable<IJavaScriptStatement> GetJavaScript(HttpContext httpContext)
        {
            yield return new SetTabCompletionCommands(GetTabCompletionCommands(httpContext));
            yield return new SetUploadEnabled(IsUploadEnabled(httpContext));
        }

        protected virtual IEnumerable<string> GetTabCompletionCommands(HttpContext httpContext)
        {
            ICommandProvider commands = httpContext.RequestServices.GetRequiredService<ICommandProvider>();
            ICommandExecutionEnvironment environment = httpContext.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

            foreach (ICommand cmd in commands.GetCommands(httpContext))
            {
                if (environment.IsTabCompletionEnabled(cmd, httpContext))
                {
                    CommandInfo info = CommandInfo.Get(cmd);
                    yield return info.Name;
                }
            }
        }

        protected virtual bool IsUploadEnabled(HttpContext httpContext)
        {
            CommandInfo info = CommandInfo.Get(typeof(Upload));
            CommandDefinition definition = _options.BuiltInCommands[info.Name];
            return definition.IsEnabled;
        }
    }
}
