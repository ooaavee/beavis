using System;
using System.Threading.Tasks;
using BeavisCli.Internal;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;

namespace BeavisCli
{
    public abstract class AbstractApplication
    {
        private ApplicationInfo _info;

        public abstract ApplicationInfo GetInfo();

        public abstract Task ExecuteAsync(ICommandLineApplication app, CliContext context);

        protected Task<int> ExitWithHelp(ICommandLineApplication cli)
        {
            var target = FindTarget(cli);
            target.ShowHelp(Info.Name);
            return Task.FromResult(2);
        }

        protected Task<int> Exit()
        {
            return Task.FromResult(2);
        }

        protected async Task OnExecuteAsync(Func<Task<int>> invoke, ICommandLineApplication app, CliContext context)
        {
            if (invoke == null)
            {
                throw new ArgumentNullException(nameof(invoke));
            }

            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string[] args = context.Request.GetArgs();
            CommandLineApplication cli = FindTarget(app);
            await cli.OnExecuteAsync(invoke);
            cli.Execute(args);
        }

        internal ApplicationInfo Info
        {
            get { return _info ?? (_info = GetInfo()); }
        }

        internal async Task ExecuteAsync(CliContext context)
        {
            var target = new CommandLineApplication
            {
                Name = Info.Name,
                FullName = Info.Name,
                Description = Info.Description,
                Out = context.Response.CreateTextWriterForInformationMessages(),
                Error = context.Response.CreateTextWriterForErrorMessages()
            };

            target.HelpOption("-?|-h|--help");

            ICommandLineApplication app = new DefaultCommandLineApplication(target);

            await ExecuteAsync(app, context);
        }

        private CommandLineApplication FindTarget(ICommandLineApplication app)
        {
            var impl = app as DefaultCommandLineApplication;
            if (impl == null)
            {
                throw new InvalidOperationException($"Cannot find the {nameof(DefaultCommandLineApplication)} object, operation terminated...");
            }
            return impl.Target;
        }
    }
}