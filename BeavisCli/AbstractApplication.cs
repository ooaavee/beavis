using System;
using System.Threading.Tasks;
using BeavisCli.Internal;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;

namespace BeavisCli
{
    public abstract class AbstractApplication
    {
        public ICommandLineApplication CreateCommandLineApplication(CliContext context)
        {
            var info = GetInfo();

            var app = new CommandLineApplication
            {
                Name = info.Name,
                FullName = info.Name,
                Description = info.Description,
                Out = context.Response.CreateTextWriterForInformationMessages(),
                Error = context.Response.CreateTextWriterForErrorMessages()
            };

            app.HelpOption("-?|-h|--help");

            return new DefaultCommandLineApplication(app, this);
        }

        public abstract ApplicationInfo GetInfo();

        public abstract Task ExecuteAsync(ICommandLineApplication app, CliContext context);



        protected async Task<int> ExitWithHelp(ICommandLineApplication cli)
        {
            var info = GetInfo();
            var target = FindCli(cli);
            target.ShowHelp(info.Name);
            return await Exit();
        }

        protected Task<int> Exit()
        {
            return Task.FromResult(2);
        }

        protected async Task OnExecuteAsync(Func<Task<int>> invoke, ICommandLineApplication app, CliContext context)
        {
            string[] args = context.Request.ParseArgs();
            CommandLineApplication cli = FindCli(app);
            await cli.OnExecuteAsync(invoke);
            cli.Execute(args);
        }

        private CommandLineApplication FindCli(ICommandLineApplication app)
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