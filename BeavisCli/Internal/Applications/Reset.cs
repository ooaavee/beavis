using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplicationDefinition(Name = "reset", Description = "Resets the terminal.")]
    internal class Reset : WebCliApplication
    {
        private readonly IHostingEnvironment _env;

        public Reset(IHostingEnvironment env)
        {
            _env = env;
        }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                context.Response.AddStatement(new ClearTerminal());
                context.Response.AddStatement(new ClearTerminalHistory());
                return Exit(context);
            }, context);

        }
    }
}
