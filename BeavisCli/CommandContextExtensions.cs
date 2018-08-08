using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli
{
    public static class CommandContextExtensions
    {
        private const int ExitStatusCode = 2;

        public static async Task OnExecuteAsync(this CommandContext context, Func<Task<int>> invoke)
        {
            // register invoke hook
            context.Processor.Invoke = () => invoke().Result;

            // get arguments for the command
            string[] args = context.Request.GetCommandArgs();

            context.Logger().LogDebug($"Started to execute '{context.Command.GetType().FullName}' with arguments '{string.Join(" ", args)}'.");

            // execute the command
            int statusCode = context.Processor.Execute(args);

            context.Logger().LogDebug($"'{context.Command.GetType().FullName}' execution completed with status code {statusCode}.");

            await Task.CompletedTask;
        }

        public static ICommandOption Option(this CommandContext context, string template, string description, CommandOptionType optionType)
        {
            Microsoft.Extensions.CommandLineUtils.CommandOptionType cot;

            switch (optionType)
            {
                case CommandOptionType.MultipleValue:
                    cot = Microsoft.Extensions.CommandLineUtils.CommandOptionType.MultipleValue;
                    break;

                case CommandOptionType.SingleValue:
                    cot = Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue;
                    break;

                case CommandOptionType.NoValue:
                    cot = Microsoft.Extensions.CommandLineUtils.CommandOptionType.NoValue;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(optionType), optionType, null);
            }

            return new CommandOption(context.Processor.Option(template, description, cot));
        }

        public static ICommandArgument Argument(this CommandContext context, string name, string description, bool multipleValues = false)
        {
            return new CommandArgument(context.Processor.Argument(name, description, multipleValues));
        }
     
        public static Task<int> Exit(this CommandContext context)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}'.");
            return Task.FromResult(ExitStatusCode);
        }

        public static async Task<int> ExitAsync(this CommandContext context)
        {
            int result = context.Exit().Result;
            return await Task.FromResult(result);
        }

        public static Task<int> Exit(this CommandContext context, string text, ResponseMessageTypes type)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with message type '{type}' and text '{text}'.");

            switch (type)
            {
                case ResponseMessageTypes.Information:
                    context.Response.WriteInformation(text);
                    break;
                case ResponseMessageTypes.Error:
                    context.Response.WriteError(text);
                    break;
                case ResponseMessageTypes.Success:
                    context.Response.WriteSuccess(text);
                    break;
            }

            return Task.FromResult(ExitStatusCode);
        }

        public static async Task<int> ExitAsync(this CommandContext context, string text, ResponseMessageTypes type)
        {
            int result = context.Exit(text, type).Result;
            return await Task.FromResult(result);
        }

        public static Task<int> ExitWithHelp(this CommandContext context)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with help.");

            if (context.Response.Messages.Any())
            {
                context.Response.WriteEmptyLine();
            }

            context.Processor.ShowHelp(context.Info.Name);
            return Task.FromResult(ExitStatusCode);
        }

        public static async Task<int> ExitWithHelpAsync(this  CommandContext context)
        {
            int result = context.ExitWithHelp().Result;
            return await Task.FromResult(result);
        }

        public static Task<int> ExitWithUnauthorized(this CommandContext context)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with unauthorized.");
            IUnauthorizedHandler handler = context.HttpContext.RequestServices.GetRequiredService<IUnauthorizedHandler>();
            handler.OnUnauthorizedAsync(context);
            return Task.FromResult(ExitStatusCode);
        }

        public static async Task<int> ExitWithUnauthorizedAsync(this CommandContext context)
        {
            int result = context.ExitWithUnauthorized().Result;
            return await Task.FromResult(result);
        }

        public static Task<int> ExitWithError(this  CommandContext context, string text)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with error '{text}'.");
            context.Response.WriteError(text);
            return Task.FromResult(ExitStatusCode);
        }

        public static async Task<int> ExitWithErrorAsync(this CommandContext context, string text)
        {
            int result = context.ExitWithError(text).Result;
            return await Task.FromResult(result);
        }

        public static Task<int> ExitWithError(this CommandContext context, string text, Exception e)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with error '{text}' and exception '{e}'.");
            context.Response.WriteError(text);
            context.Response.WriteError(e, true);
            return Task.FromResult(ExitStatusCode);
        }

        public static async Task<int> ExitWithErrorAsync(this  CommandContext context, string text, Exception e)
        {
            int result = context.ExitWithError(text, e).Result;
            return await Task.FromResult(result);
        }

        public static Task<int> ExitWithError(this CommandContext context, Exception e)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with exception '{e}'.");
            context.Response.WriteError(e, true);
            return Task.FromResult(ExitStatusCode);
        }

        public static async Task<int> ExitWithErrorAsync(this  CommandContext context, Exception e)
        {
            int result = context.ExitWithError(e).Result;
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Checks if the command is built-in.
        /// </summary>
        public static bool IsBuiltInCommand(this CommandContext context)
        {
            return context.Command.GetType().Assembly.Equals(typeof(ICommand).Assembly);
        }

        public static ILogger Logger(this CommandContext context)
        {
            if (context.Items.TryGetValue("logger", out var tmp))
            {
                return (ILogger)tmp;
            }

            ILoggerFactory loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger<CommandContext>();
            context.Items["logger"] = logger;
            return logger;
        }        
    }
}
