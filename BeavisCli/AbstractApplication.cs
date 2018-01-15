using BeavisCli.Internal;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    public abstract class AbstractApplication
    {
        private const int ExitStatusCode = 2;

        public abstract ApplicationInfo GetInfo();

        public virtual Task<AuthorizeResult> OnAuthorize(ApplicationExecutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Task.FromResult((AuthorizeResult) new AuthorizedByDefaultAuthorizeResult());
        }

        public abstract Task ExecuteAsync(ApplicationExecutionContext context);

        protected async Task OnExecuteAsync(Func<Task<int>> invoke, ApplicationExecutionContext context)
        {
            if (invoke == null)
            {
                throw new ArgumentNullException(nameof(invoke));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string[] args = context.Request.GetArgs();
            CommandLineApplication cli = FindCli(context.Host);
            await cli.OnExecuteAsync(invoke);
            cli.Execute(args);
        }
      
        protected Task<int> Exit(ApplicationExecutionContext context)
        {
            return Task.FromResult(ExitStatusCode);
        }

        protected Task<int> ExitWithHelp(ApplicationExecutionContext context)
        {
            CommandLineApplication cli = FindCli(context.Host);
            cli.ShowHelp(context.Info.Name);
            return Task.FromResult(ExitStatusCode);
        }

        protected Task<int> Unauthorized(ApplicationExecutionContext context)
        {
            IOptions<BeavisCliOptions> options = context.HttpContext.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>();
            if (options.Value.UnauthorizedApplicationExecutionAttemptHandler != null)
            {
                options.Value.UnauthorizedApplicationExecutionAttemptHandler.HandleUnauthorizedApplicationExecution(context);
            }
            return Task.FromResult(ExitStatusCode);
        }

        private CommandLineApplication FindCli(ICommandLineApplication host)
        {
            if (!(host is DefaultCommandLineApplication obj))
            {
                throw new InvalidOperationException($"Cannot find the {nameof(DefaultCommandLineApplication)} object, operation terminated...");
            }
            return obj.Cli;
        }

    }
}