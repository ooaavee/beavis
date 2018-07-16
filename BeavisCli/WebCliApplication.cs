using BeavisCli.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BeavisCli
{
    public abstract class WebCliApplication
    {        
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

        private ILogger<WebCliApplication> _logger;

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

            _logger = context.GetLoggerFactory().CreateLogger<WebCliApplication>();

                       

            string[] args = context.Request.GetApplicationArgs();

            await context.Cli.OnExecuteAsync(invoke);

            context.Cli.Execute(args);
        }
      
        protected Task<int> Exit(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Exit();
        }

        protected Task<int> ExitWithHelp(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Cli.ShowHelp(this.GetInfo().Name);

            return Exit();
        }

        protected Task<int> Unauthorized(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.GetUnauthorizedHandler().OnUnauthorizedAsync(context);

            return Exit();
        }

        private Task<int> Exit()
        {
            const int exitStatusCode = 2;
            return Task.FromResult(exitStatusCode);
        }
    }
}