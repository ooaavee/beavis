using BeavisCli.Internal;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BeavisCli
{
    public abstract class WebCliApplication
    {
        private const int ExitStatusCode = 2;

        private ILogger<WebCliApplication> _logger;

        /// <summary>
        /// Checks if the application execution is authorized.
        /// </summary>
        public virtual bool IsAuthorized(WebCliContext context)
        {
            return true;
        }

        /// <summary>
        /// Checks if the application is browsable.
        /// </summary>
        public virtual bool IsBrowsable(WebCliContext context)
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
            await context.Cli.OnExecuteAsync(invoke);

            // create logger
            _logger = context.GetLoggerFactory().CreateLogger<WebCliApplication>();
                       
            // get arguments for the application
            string[] args = context.Request.GetApplicationArgs();

            _logger.LogDebug($"Started to execute '{GetType().FullName}' with arguments '{string.Join(" ", args)}'.");

            // execute the application
            int statusCode = context.Cli.Execute(args);

            _logger.LogDebug($"'{GetType().FullName}' execution completed with status code {statusCode}.");
        }

        protected Task<int> Exit(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}'.");

            return Exit();
        }

        protected Task<int> ExitWithHelp(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with help.");

            WebCliApplicationInfo info = this.GetInfo();

            context.Cli.ShowHelp(info.Name);

            return Exit();
        }

        protected Task<int> Unauthorized(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _logger?.LogDebug($"Exiting '{GetType().FullName}' with unauthorized.");

            context.GetUnauthorizedHandler().OnUnauthorizedAsync(context);
            
            return Exit();
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

            return Exit();
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

            return Exit();
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
            return Exit();
        }

        private static Task<int> Exit()
        {
            return Task.FromResult(ExitStatusCode);
        }
    }
}