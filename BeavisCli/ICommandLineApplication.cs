using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{


    public class RequestHandler
    {
        public async Task HandleAsync(TerminalRequest request, HttpContext httpContext)
        {
            TerminalExecutionContext context = null;

            IBeavisApplication beavisApplication = null;
            ICommandLineApplication cli = beavisApplication.CreateCommandLineApplication(context);

            try
            {
                beavisApplication.OnRunAsync(cli, context);
            }
            catch (CommandParsingException ex)
            {
                using (context.Response.BeginInteraction())
                {
                    context.Response.WriteError(ex);
                }
            }

        }
    }

    public interface IBeavisApplication
    {
        BeavisApplicationInfo GetInfo();

        ICommandLineApplication CreateCommandLineApplication(TerminalExecutionContext context);

        Task OnRunAsync(ICommandLineApplication cli, TerminalExecutionContext context);

    }

    public abstract class BeavisApplication : IBeavisApplication
    {
        public ICommandLineApplication CreateCommandLineApplication(TerminalExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public abstract BeavisApplicationInfo GetInfo();

        public async Task OnRunAsync(ICommandLineApplication cli, TerminalExecutionContext context)
        {
            ICommandOption opt1 = cli.Option("-opt1", "Description", CommandOptionType.SingleValue);

            await this.ExecuteAsyncX(() =>
            {
                string sss = null;

                return ExitWithHelp(cli);
                //return Exit();

            }, cli, context);
           

//            return Task.CompletedTask;
        }

        protected  async Task<int> ExitWithHelp(ICommandLineApplication cli)
        {
            var info = GetInfo();
            var target = FindTarget(cli);
            target.ShowHelp(info.Name);
            return await Exit();
        }

        protected Task<int> Exit()
        {
            return Task.FromResult(2);
        }

        private CommandLineApplication FindTarget(ICommandLineApplication cli)
        {
            DefaultCommandLineApplication impl = cli as DefaultCommandLineApplication;
            CommandLineApplication target = impl.Target;
            return target;
        }

        protected async Task ExecuteAsyncX(Func<Task<int>> invoke, ICommandLineApplication cli, TerminalExecutionContext context)
        {
            CommandLineApplication target = FindTarget(cli);
            await target.OnExecuteAsync(invoke);

            string[] args = context.Request.GetApplicationArgs();

            target.Execute(args);                      
        }




        //public void OnExecute(Func<int> invoke)
        //{
        //    Invoke = invoke;
        //}


    }

    public interface ICommandLineApplication
    {
        ICommandOption Option(string template, string description, CommandOptionType optionType);
        ICommandArgument Argument(string name, string description, bool multipleValues = false);


  

    }

    internal class DefaultCommandLineApplication : ICommandLineApplication
    {
        private readonly CommandLineApplication _cli;
        private readonly IBeavisApplication _beavisApplication;

        public DefaultCommandLineApplication(CommandLineApplication cli, IBeavisApplication beavisApplication)
        {
            _cli = cli;
            _beavisApplication = beavisApplication;
        }

        public CommandLineApplication Target
        {
            get { return _cli; }
        }

        public ICommandOption Option(string template, string description, CommandOptionType optionType)
        {
            throw new NotImplementedException();
        }

        public ICommandArgument Argument(string name, string description, bool multipleValues = false)
        {
            throw new NotImplementedException();
        }

    }


    public interface ICommandOption
    {
        List<string> Values { get; }
        bool HasValue();
        string Value();
    }

    public interface ICommandArgument
    {
        List<string> Values { get; }
        bool MultipleValues { get; }
        string Value { get; }
    }

    public enum CommandOptionType
    {
        MultipleValue,
        SingleValue,
        NoValue
    }


}