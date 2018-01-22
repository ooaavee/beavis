using System;
using System.Collections.Generic;
using System.Reflection;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli.Internal
{
    internal class DefaultTerminalInitializer : ITerminalInitializer
    {
        public void Initialize(HttpContext context, WebCliResponse response)
        {
            // Display app name and some copyright stuff.  
            //
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            string version = $"{name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";

            response.WriteSuccess($"{product.Product} {version}");
            response.WriteSuccess($"{copyright.Copyright}. MIT License.");

            // Tabcompletion
            //
            var sandbox = context.RequestServices.GetRequiredService<WebCliSandbox>();
            var applications = new List<string>();
            foreach (var application in sandbox.GetApplications(context))
            {
                applications.Add(application.Name);
            }
            response.AddStatement(new SetTerminalCompletionDictionary(applications.ToArray()));
        }
    }
}
