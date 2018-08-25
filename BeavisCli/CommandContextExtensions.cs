using BeavisCli.JavaScriptStatements;
using BeavisCli.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BeavisCli
{
    public delegate Task<CommandResult> StringAnswerHandler(string result, CommandContext context);

    public delegate Task<CommandResult> BooleanAnswerHandler(bool result, CommandContext context);

    public static class CommandContextExtensions
    {
        public static async Task OnExecuteAsync(this CommandContext context, Func<Task<CommandResult>> invoke)
        {
            // invoke hook
            context.Processor.Invoke = () => invoke().Result.StatusCode;

            // get arguments for the command
            string[] args = context.Request.GetCommandArgs();

            context.Logger().LogDebug($"Started to execute '{context.Command.GetType().FullName}' with arguments '{string.Join(" ", args)}'.");

            // execute the command
            int statusCode = context.Processor.Execute(args);

            context.Logger().LogDebug($"'{context.Command.GetType().FullName}' execution completed with status code {statusCode}.");

            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new command options.
        /// </summary>
        public static ICommandOption Option(this CommandContext context, string template, string description, CommandOptionType optionType)
        {
            Microsoft.Extensions.CommandLineUtils.CommandOptionType o;

            switch (optionType)
            {
                case CommandOptionType.MultipleValue:
                    o = Microsoft.Extensions.CommandLineUtils.CommandOptionType.MultipleValue;
                    break;
                case CommandOptionType.SingleValue:
                    o = Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue;
                    break;
                case CommandOptionType.NoValue:
                    o = Microsoft.Extensions.CommandLineUtils.CommandOptionType.NoValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionType), optionType, null);
            }

            return new CommandOption(context.Processor.Option(template, description, o));
        }

        /// <summary>
        /// Creates a new command argument.
        /// </summary>
        public static ICommandArgument Argument(this CommandContext context, string name, string description, bool multipleValues = false)
        {
            return new CommandArgument(context.Processor.Argument(name, description, multipleValues));
        }

        /// <summary>
        /// Checks if the command is built-in.
        /// </summary>
        public static bool IsBuiltInCommand(this CommandContext context)
        {
            return context.Command.GetType().Assembly.Equals(typeof(ICommand).Assembly);
        }
       
        /// <summary>
        /// Writes a plain text message.
        /// </summary>
        public static void WriteText(this CommandContext context, string text)
        {
            context.Response.Messages.Add(ResponseMessage.Plain(text));
        }

        /// <summary>
        /// Writes plain text messages.
        /// </summary>
        public static void WriteText(this CommandContext context, IEnumerable<string> texts)
        {
            foreach (string text in texts)
            {
                context.WriteText(text);
            }
        }

        /// <summary>
        /// Writes a success/ very positive message.
        /// </summary>
        public static void WriteSuccess(this CommandContext context, string text)
        {
            context.Response.Messages.Add(ResponseMessage.Success(text));
        }

        /// <summary>
        /// Writes success/ very positive messages.
        /// </summary>
        public static void WriteSuccess(this CommandContext context, IEnumerable<string> texts)
        {
            foreach (string text in texts)
            {
                context.WriteSuccess(text);
            }
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public static void WriteError(this CommandContext context, Exception e, bool returnStackTrace = false)
        {
            string text = returnStackTrace ? e.ToString() : e.Message;

            context.WriteError(text);
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public static void WriteError(this CommandContext context, string text)
        {
            context.Response.Messages.Add(ResponseMessage.Error(text));
        }

        /// <summary>
        /// Writes error messages.
        /// </summary>
        public static void WriteError(this CommandContext context, IEnumerable<string> texts)
        {
            foreach (string text in texts)
            {
                context.WriteError(text);
            }
        }

        /// <summary>
        /// Writes a JavaScript statement that will be invoked on the client-side.
        /// </summary>
        public static void WriteJs(this CommandContext context, IJavaScriptStatement js)
        {
            string code = js.GetCode();

            context.Response.Statements.Add(code);
        }

        /// <summary>
        /// Writes JavaScript statements that will be invoked on the client-side.
        /// </summary>
        public static void WriteJs(this CommandContext context, IEnumerable<IJavaScriptStatement> js)
        {
            foreach (IJavaScriptStatement j in js)
            {
                context.WriteJs(j);
            }
        }

        /// <summary>
        /// Writes a file.
        /// </summary>
        public static void WriteFile(this CommandContext context, byte[] content, string fileName, string mimeType)
        {
            IJob job = new WriteFileJob(content, fileName, mimeType);

            context.AddJob(job);
        }
     
        public static void WriteObjects<TObj, TMember1, TMember2>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3, TMember4>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    expression4,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3, TMember4, TMember5>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    expression4,
                    expression5,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    expression4,
                    expression5,
                    expression6,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    expression4,
                    expression5,
                    expression6,
                    expression7,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TMember8>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            Expression<Func<TObj, TMember8>> expression8,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    expression4,
                    expression5,
                    expression6,
                    expression7,
                    expression8,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TMember8, TMember9>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            Expression<Func<TObj, TMember8>> expression8,
            Expression<Func<TObj, TMember9>> expression9,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    expression4,
                    expression5,
                    expression6,
                    expression7,
                    expression8,
                    expression9,
                    indent,
                    createHeader));
        }

        public static void WriteObjects<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TMember8, TMember9, TMember10>(
            this CommandContext context,
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            Expression<Func<TObj, TMember8>> expression8,
            Expression<Func<TObj, TMember9>> expression9,
            Expression<Func<TObj, TMember10>> expression10,
            bool indent,
            bool createHeader)
        {
            context.WriteText(
                LineFormatter.CreateLines(
                    objects,
                    expression1,
                    expression2,
                    expression3,
                    expression4,
                    expression5,
                    expression6,
                    expression7,
                    expression8,
                    expression9,
                    expression10,
                    indent,
                    createHeader));
        }

        public static Task<CommandResult> Exit(this CommandContext context)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}'.");
            return Task.FromResult(CommandResult.Default);
        }

        public static async Task<CommandResult> ExitAsync(this CommandContext context)
        {
            CommandResult result = context.Exit().Result;
            return await Task.FromResult(result);
        }

        public static Task<CommandResult> Exit(this CommandContext context, string text, ResponseMessageTypes type = ResponseMessageTypes.Plain)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with message type '{type}' and text '{text}'.");

            switch (type)
            {
                case ResponseMessageTypes.Plain:
                    context.WriteText(text);
                    break;
                case ResponseMessageTypes.Error:
                    context.WriteError(text);
                    break;
                case ResponseMessageTypes.Success:
                    context.WriteSuccess(text);
                    break;
            }

            return Task.FromResult(CommandResult.Default);
        }

        public static async Task<CommandResult> ExitAsync(this CommandContext context, string text, ResponseMessageTypes type = ResponseMessageTypes.Plain)
        {
            CommandResult result = context.Exit(text, type).Result;
            return await Task.FromResult(result);
        }

        public static Task<CommandResult> ExitWithHelp(this CommandContext context)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with help.");

            if (context.Response.Messages.Any())
            {
                context.WriteText("");
            }

            context.Processor.ShowHelp(context.Info.Name);
            return Task.FromResult(CommandResult.Default);
        }

        public static async Task<CommandResult> ExitWithHelpAsync(this CommandContext context)
        {
            CommandResult result = context.ExitWithHelp().Result;
            return await Task.FromResult(result);
        }

        public static Task<CommandResult> ExitWithUnauthorized(this CommandContext context)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with unauthorized.");
            IUnauthorizedHandler handler = context.HttpContext.RequestServices.GetRequiredService<IUnauthorizedHandler>();
            handler.OnUnauthorizedAsync(context);
            return Task.FromResult(CommandResult.Default);
        }

        public static async Task<CommandResult> ExitWithUnauthorizedAsync(this CommandContext context)
        {
            CommandResult result = context.ExitWithUnauthorized().Result;
            return await Task.FromResult(result);
        }

        public static Task<CommandResult> ExitWithError(this CommandContext context, string text)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with error '{text}'.");
            context.WriteError(text);
            return Task.FromResult(CommandResult.Default);
        }

        public static async Task<CommandResult> ExitWithErrorAsync(this CommandContext context, string text)
        {
            CommandResult result = context.ExitWithError(text).Result;
            return await Task.FromResult(result);
        }

        public static Task<CommandResult> ExitWithError(this CommandContext context, string text, Exception e)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with error '{text}' and exception '{e}'.");
            context.WriteError(text);
            context.WriteError(e, true);
            return Task.FromResult(CommandResult.Default);
        }

        public static async Task<CommandResult> ExitWithErrorAsync(this CommandContext context, string text, Exception e)
        {
            CommandResult result = context.ExitWithError(text, e).Result;
            return await Task.FromResult(result);
        }

        public static Task<CommandResult> ExitWithError(this CommandContext context, Exception e)
        {
            context.Logger().LogDebug($"Exiting '{context.Command.GetType().FullName}' with exception '{e}'.");
            context.WriteError(e, true);
            return Task.FromResult(CommandResult.Default);
        }

        public static async Task<CommandResult> ExitWithErrorAsync(this CommandContext context, Exception e)
        {
            CommandResult result = context.ExitWithError(e).Result;
            return await Task.FromResult(result);
        }

        public static Task<CommandResult> AskText(this CommandContext context, string question, StringAnswerHandler handler)
        {
            return AskQuestion(context, new []{ question }, false, handler, null);
        }

        public static Task<CommandResult> AskPassword(this CommandContext context, string question, StringAnswerHandler handler)
        {
            return AskQuestion(context, new[] { question }, true, handler, null);
        }

        public static Task<CommandResult> AskConfirmation(this CommandContext context, string question, BooleanAnswerHandler handler)
        {
            var questions = new[]
            {
                question,
                "Only 'yes' will be accepted to approve."
            };

            return AskQuestion(context, questions, false, null, handler);
        }

        private static Task<CommandResult> AskQuestion(CommandContext context, string[] questions, bool useMask, StringAnswerHandler stringHandler, BooleanAnswerHandler booleanHandler)
        {
            // this will be invoked just before we are sending the response
            context.Response.Sending += (sender, args) =>
            {
                IJobPool pool = context.HttpContext.RequestServices.GetRequiredService<IJobPool>();

                IJob job;
                if (stringHandler != null)
                {
                     job = new HandleAnswerJob(context, stringHandler);
                }
                else if (booleanHandler != null)
                {
                    job = new HandleAnswerJob(context, booleanHandler);
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(StringAnswerHandler)} or {nameof(BooleanAnswerHandler)} must be set.");
                }

                string key = pool.Push(job);

                foreach (string question in questions)
                {
                    context.WriteText(question);
                }

                // disable terminal prompt
                context.WriteJs(new SetPrompt(""));

                // disable terminal history -> this will set back to 'enabled' in HandleAnswerJob
                context.WriteJs(new SetHistory(false));

                if (useMask)
                {
                    context.WriteJs(new SetMask(true));
                    context.WriteJs(new QueueJob(key, new SetMask(false)));
                }
                else
                {
                    context.WriteJs(new QueueJob(key));
                }
            };

            return Task.FromResult(CommandResult.Default);
        }

        /// <summary>
        /// Adds a job.
        /// </summary>
        public static void AddJob(this CommandContext context, IJob job)
        {
            // this will be invoked just before we are sending the response
            context.Response.Sending += (sender, args) =>
            {
                // push a new job into the pool and add a JavaScript statement that
                // begins the job on the client-side
                IJobPool pool = context.HttpContext.RequestServices.GetRequiredService<IJobPool>();
                string key = pool.Push(job);
                IJavaScriptStatement js = new BeginJob(key);
                context.WriteJs(js);
            };
        }

        private static ILogger Logger(this CommandContext context)
        {
            const string key = "__BeavisCli.CommandContext.Logger";

            if (context.HttpContext.Items.TryGetValue(key, out var tmp))
            {
                return (ILogger) tmp;
            }

            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<CommandContext>();
            context.HttpContext.Items[key] = logger;
            return logger;
        }
    }
}
