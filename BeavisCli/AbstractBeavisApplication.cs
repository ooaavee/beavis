﻿using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    internal abstract class AbstractBeavisApplication
    {
        public Func<BeavisApplicationInfo> GetInfo { get; set; }

        public async Task RunAsync(TerminalExecutionContext context)
        {
            ////if (true)
            ////{
                try
                {
                    await OnRunAsync(context);
                }
                catch (CommandParsingException ex)
                {
                    using (context.Response.BeginInteraction())
                    {
                        context.Response.WriteError(ex);
                    }
                }
            ////}
            ////else
            ////{
            ////    using (context.Response.BeginInteraction())
            ////    {
            ////        context.Response.WriteError("Unauthorized");
            ////    }
            ////}
        }      

        protected abstract Task OnRunAsync(TerminalExecutionContext context);

        protected CommandLineApplication CreateApplication(BeavisApplicationInfo info, TerminalExecutionContext context)
        {
            var app = new CommandLineApplication
            {
                Name = info.Name,
                FullName = info.Name,
                Description = info.Description,
                Out = context.Response.CreateTextWriterForInformationMessages(),
                Error = context.Response.CreateTextWriterForErrorMessages()
            };

            app.HelpOption("-?|-h|--help");

            return app;
        }

        protected static bool HasArgument(CommandArgument arg)
        {
            if (arg == null)
            {
                return false;
            }

            if (!arg.MultipleValues && !string.IsNullOrEmpty(arg.Value))
            {
                return true;
            }

            return false;
        }

        protected static bool HasOption(CommandOption option)
        {
            if (option == null)
            {
                return false;
            }
           
            if (option.Values.Count == 1 && !string.IsNullOrEmpty(option.Value()))
            {
                return true;
            }

            return false;
        }

        protected static string GetValue(CommandArgument arg)
        {
            if (!HasArgument(arg))
            {
                throw new InvalidOperationException("Argument value is not available.");
            }

            string value = arg.Value;
            return value;
        }

        protected static string GetValue(CommandOption options)
        {
            if (!HasOption(options))
            {
                throw new InvalidOperationException("Option value is not available.");
            }

            string value = options.Value();
            return value;
        }

        protected static string GetIsMandatoryError(CommandOption option)
        {
            string error = string.Format("The option '{0}' is mandatory", option.Template);
            return error;
        }


        protected void Execute(CommandLineApplication app, TerminalExecutionContext context)
        {
            string[] args = context.Request.GetApplicationArgs();
            app.Execute(args);
        }

        protected int Exit()
        {
            return 2;
        }
        
    }
}