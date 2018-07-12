using BeavisCli.Internal;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace BeavisCli
{
    public class DefaultTerminalInitializer : ITerminalInitializer
    {
        public virtual void Initialize(HttpContext context, WebCliResponse response)
        {
            PrepareTabCompletion(context, response);
            SetVariables(context, response);
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
