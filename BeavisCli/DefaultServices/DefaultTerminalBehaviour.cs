﻿using BeavisCli.Internal;
using BeavisCli.Internal.Commands;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeavisCli.DefaultServices
{
    public class DefaultTerminalBehaviour : ITerminalBehaviour
    {
        public virtual void OnInitialize(HttpContext context, WebCliResponse response)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            foreach (IJavaScriptStatement js in GetJavaScriptStatements(context))
            {
                response.AddJavaScript(js);
            }

            foreach (string greeting in GetGreetings(context))
            {
                response.WriteSuccess(greeting);
            }
        }

        public virtual bool IsVisibleForHelp(WebCliCommand cmd, WebCliContext context)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            WebCliOptions options = context.HttpContext.RequestServices.GetRequiredService<IOptions<WebCliOptions>>().Value;

            if (cmd.GetType() == typeof(Help))
            {
                // ignore 'help'
                return false;
            }

            if (cmd.IsBuiltIn)
            {
                BuiltInCommandDefinition definition = options.BuiltInCommands[cmd.Info.Name];
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

        protected virtual IEnumerable<string> GetGreetings(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            string line1 = $"{product.Product} {name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";
            string line2 = $"{copyright.Copyright}. Code released under the MIT License. Usage 'license' for more details.";

            yield return line1;
            yield return line2;
        }

        protected virtual IEnumerable<IJavaScriptStatement> GetJavaScriptStatements(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            yield return new SetTerminalCompletionDictionary(GetTabCompletionCommands(context));
            yield return new SetUploadEnabled(IsUploadEnabled(context));
        }

        protected virtual IEnumerable<string> GetTabCompletionCommands(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            WebCliSandbox sandbox = context.RequestServices.GetRequiredService<WebCliSandbox>();

            foreach (WebCliCommand cmd in sandbox.GetCommands(context))
            {
                if (IsTabCompletionEnabled(cmd, context))
                {
                    yield return cmd.Info.Name;
                }
            }
        }

        protected virtual bool IsTabCompletionEnabled(WebCliCommand cmd, HttpContext context)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            WebCliOptions options = context.RequestServices.GetRequiredService<IOptions<WebCliOptions>>().Value;

            bool enabled = false;

            if (cmd.IsBuiltIn)
            {
                BuiltInCommandDefinition definition = options.BuiltInCommands[cmd.Info.Name];

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

        protected virtual bool IsUploadEnabled(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            WebCliOptions options = context.RequestServices.GetRequiredService<IOptions<WebCliOptions>>().Value;
            WebCliCommandInfo info = WebCliCommandInfo.FromType(typeof(Upload));
            BuiltInCommandDefinition definition = options.BuiltInCommands[info.Name];
            return definition.IsEnabled;
        }
    }
}
