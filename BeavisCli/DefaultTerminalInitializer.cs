using BeavisCli.Internal;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace BeavisCli
{
    public class DefaultTerminalInitializer : ITerminalInitializer
    {
        public virtual void Initialize(HttpContext context, WebCliResponse response)
        {
            DisplayInfoText(context, response);
            PrepareTabCompletion(context, response);
            SetVariables(context, response);
        }

        protected virtual void DisplayInfoText(HttpContext context, WebCliResponse response)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            string version = $"{name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";

            response.WriteSuccess($"{product.Product} {version}");
            response.WriteSuccess($"{copyright.Copyright}. MIT License.");
        }

        protected virtual void PrepareTabCompletion(HttpContext context, WebCliResponse response)
        {
            var sandbox = context.RequestServices.GetRequiredService<WebCliSandbox>();
            var applications = new List<string>();
            foreach (var application in sandbox.GetApplications(context))
            {
                applications.Add(application.Name);
            }
            response.AddStatement(new SetTerminalCompletionDictionary(applications.ToArray()));
        }

        protected virtual void SetVariables(HttpContext context, WebCliResponse response)
        {
            IOptions<WebCliOptions> options = context.RequestServices.GetRequiredService<IOptions<WebCliOptions>>();
            bool enabled = options.Value.EnableFileUpload;
            response.AddStatement(new SetUploadEnabled(enabled));
        }
    }
}
