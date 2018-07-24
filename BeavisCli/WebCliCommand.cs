using BeavisCli.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace BeavisCli
{
    public abstract class WebCliCommand
    {
        private static readonly Assembly ThisAssembly = typeof(WebCliCommand).Assembly;

        // we can safely use a static dictionary cache here, because these values doesn't change during runtime
        private static readonly ConcurrentDictionary<Type, WebCliCommandInfo> InfoCache = new ConcurrentDictionary<Type, WebCliCommandInfo>();

        private const int ExitStatusCode = 2;

        private ILogger<WebCliCommand> _logger;

        /// <summary>
        /// Checks if the command execution is authorized.
        /// </summary>
        public virtual bool IsAuthorized(WebCliContext context)
        {
            return true;
        }

        /// <summary>
        /// Checks if the command is visible for 'help'.
        /// </summary>
        public virtual bool IsVisibleForHelp(WebCliContext context)
        {
            return true;
        }

        public abstract Task ExecuteAsync(WebCliContext context);

        protected async Task OnExecuteAsync(Func<Task<int>> invoke, WebCliContext context)
        {
            if (invoke == null)
            {
                throw new ArgumentNullException(nameof(invoke));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // register invoke hook
            context.Processor.Invoke = () => invoke().Result;

            // create logger
            _logger = context.GetLoggerFactory().CreateLogger<WebCliCommand>();

            // get arguments for the command
            string[] args = context.Request.GetCommandArgs();

            _logger.LogDebug($"Started to execute '{GetType().FullName}' with arguments '{string.Join(" ", args)}'.");

            // execute the command
            int statusCode = context.Processor.Execute(args);

            _logger.LogDebug($"'{GetType().FullName}' execution completed with status code {statusCode}.");

            await Task.CompletedTask;
        }

        protected Task<int> Exit(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}'.");

            return Task.FromResult(ExitStatusCode);
        }

        protected async Task<int> ExitAsync(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            int result = Exit(context).Result;

            return await Task.FromResult(result);
        }

        protected Task<int> ExitWithHelp(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with help.");

            
            context.Processor.ShowHelp(Info.Name);

            return Task.FromResult(ExitStatusCode);
        }

        protected async Task<int> ExitWithHelpAsync(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            int result = ExitWithHelp(context).Result;

            return await Task.FromResult(result);
        }

        protected Task<int> Unauthorized(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with unauthorized.");

            context.GetUnauthorizedHandler().OnUnauthorizedAsync(context);

            return Task.FromResult(ExitStatusCode);
        }

        protected async Task<int> UnauthorizedAsync(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            int result = Unauthorized(context).Result;

            return await Task.FromResult(result);
        }

        protected Task<int> Error(WebCliContext context, string text)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with error '{text}'.");

            context.Response.WriteError(text);

            return Task.FromResult(ExitStatusCode);
        }

        protected async Task<int> ErrorAsync(WebCliContext context, string text)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            int result = Error(context, text).Result;

            return await Task.FromResult(result);
        }

        protected Task<int> Error(WebCliContext context, string text, Exception e)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with error '{text}' and exception '{e}'.");

            context.Response.WriteError(text);
            context.Response.WriteError(e, true);

            return Task.FromResult(ExitStatusCode);
        }

        protected async Task<int> ErrorAsync(WebCliContext context, string text, Exception e)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            int result = Error(context, text, e).Result;

            return await Task.FromResult(result);
        }

        protected Task<int> Error(WebCliContext context, Exception e)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with exception '{e}'.");
            context.Response.WriteError(e, true);

            return Task.FromResult(ExitStatusCode);
        }

        protected async Task<int> ErrorAsync(WebCliContext context, Exception e)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            int result = Error(context, e).Result;

            return await Task.FromResult(result);
        }

        internal WebCliCommandInfo Info => GetInfo();

        internal bool IsValid => GetInfo() != null;

        internal bool IsBuiltIn => GetType().Assembly.Equals(ThisAssembly);

        private WebCliCommandInfo GetInfo()
        {
            Type type = GetType();
            if (!InfoCache.TryGetValue(type, out WebCliCommandInfo info))
            {
                info = WebCliCommandInfo.Parse(type);
                if (info != null)
                {
                    InfoCache.TryAdd(type, info);
                }
            }
            return info;
        }
    }
}