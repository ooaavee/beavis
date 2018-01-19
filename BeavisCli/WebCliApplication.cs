using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    public abstract class WebCliApplication
    {
        private const int ExitStatusCode = 2;

        protected WebCliApplication(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public virtual bool IsAuthorized(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return true;
        }

        public virtual bool IsBrowsable(WebCliContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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

            await context.Host.Cli.OnExecuteAsync(invoke);
            context.Host.Cli.Execute(context.Request.GetArgs());
        }
      
        protected Task<int> Exit(WebCliContext context)
        {
            return Task.FromResult(ExitStatusCode);
        }

        protected Task<int> ExitWithHelp(WebCliContext context)
        {
            context.Host.Cli.ShowHelp(Name);
            return Task.FromResult(ExitStatusCode);
        }

        protected Task<int> Unauthorized(WebCliContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<WebCliOptions>>();
            options.Value.UnauthorizedHandler?.HandleUnauthorizedApplicationExecution(context);
            return Task.FromResult(ExitStatusCode);
        }

    }
}