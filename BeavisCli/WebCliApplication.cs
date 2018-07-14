using BeavisCli.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

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

            string[] args = context.Request.GetApplicationArgs();

            await context.Host.Cli.OnExecuteAsync(invoke);

            context.Host.Cli.Execute(args);
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

            context.Host.Cli.ShowHelp(this.Meta().Name);
            return Exit();
        }

        protected Task<int> Unauthorized(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IUnauthorizedHandler handler = context.HttpContext.RequestServices.GetRequiredService<IUnauthorizedHandler>();
            handler.OnUnauthorizedAsync(context);
            return Exit();
        }

        private static Task<int> Exit()
        {
            const int exitStatusCode = 2;
            return Task.FromResult(exitStatusCode);
        }
    }
}