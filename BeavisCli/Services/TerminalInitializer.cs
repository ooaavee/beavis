using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace BeavisCli.Services
{
    public class TerminalInitializer : ITerminalInitializer
    {
        public virtual void Initialize(Response response, HttpContext httpContext, bool silent = false)
        {
            if (!silent)
            {
                response.Messages.AddRange(GetMessages(httpContext));
            }

            response.AddJavaScript(GetJavaScript(httpContext));
        }

        protected virtual IEnumerable<ResponseMessage> GetMessages(HttpContext httpContext)
        {        
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            string message1 = $"{product.Product} {name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";
            yield return new SuccessMessage(message1);

            string message2 = $"{copyright.Copyright}. Code released under the MIT License. Usage 'license' for more details.";
            yield return new SuccessMessage(message2);
        }

        protected virtual IEnumerable<IJavaScriptStatement> GetJavaScript(HttpContext httpContext)
        {
            yield return new SetTabCompletionCommands(GetTabCompletionCommands(httpContext));

            IAuthorizationHandler a = httpContext.RequestServices.GetRequiredService<IAuthorizationHandler>();

            yield return new SetUploadEnabled(a.IsUploadEnabled(httpContext));
        }

        protected virtual IEnumerable<string> GetTabCompletionCommands(HttpContext httpContext)
        {
            ICommandProvider commands = httpContext.RequestServices.GetRequiredService<ICommandProvider>();
            IAuthorizationHandler authorization = httpContext.RequestServices.GetRequiredService<IAuthorizationHandler>();

            foreach (Command cmd in commands.GetCommands(httpContext))
            {
                bool enabled = authorization.IsTabCompletionEnabled(cmd, httpContext);

                if (enabled)
                {
                    yield return cmd.Info.Name;
                }
            }
        }
    }
}
