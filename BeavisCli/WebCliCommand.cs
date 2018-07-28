using BeavisCli.Internal;
using Microsoft.Extensions.DependencyInjection;
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
        private static readonly ConcurrentDictionary<Type, WebCliCommandInfo> ResolvedInfo = new ConcurrentDictionary<Type, WebCliCommandInfo>();

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

        /// <summary>
        /// Checks if the tab completion is enabled for this command.
        /// </summary>
        public virtual bool IsTabCompletionEnabled()
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
            ILoggerFactory loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<WebCliCommand>();

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
     
            return await Task.FromResult(Exit(context).Result);
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

            return await Task.FromResult(ExitWithHelp(context).Result);
        }

        protected Task<int> Unauthorized(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with unauthorized.");
            IUnauthorizedHandler handler = context.HttpContext.RequestServices.GetRequiredService<IUnauthorizedHandler>();
            handler.OnUnauthorizedAsync(this, context);
            return Task.FromResult(ExitStatusCode);
        }

        protected async Task<int> UnauthorizedAsync(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await Task.FromResult(Unauthorized(context).Result);
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

            return await Task.FromResult(Error(context, text).Result);
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

            return await Task.FromResult(Error(context, text, e).Result);
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

            return await Task.FromResult(Error(context, e).Result);
        }

        /// <summary>
        /// Gets information about this command.
        /// </summary>
        public WebCliCommandInfo Info
        {
            get
            {
                if (!ResolvedInfo.TryGetValue(GetType(), out WebCliCommandInfo value))
                {
                    if ((value = WebCliCommandInfo.FromType(GetType())) != null)
                    {
                        ResolvedInfo.TryAdd(GetType(), value);
                    }
                }
                return value;
            }
        }

        /// <summary>
        /// Checks if this built-in command.
        /// </summary>
        public bool IsBuiltIn => GetType().Assembly.Equals(ThisAssembly);
    }
}