using BeavisCli;
using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;

namespace Jemma.Terminal.Applications.Logout
{
    internal class Logout : AbstractBeavisApplication
    {
        public static readonly ApplicationInfo Definition = new ApplicationInfo
        {
            ////Type = typeof(Logout),
            Name = "logout",
            Description = "Logs out from the server."
        };


        protected override async Task OnRunAsync(CliContext context)
        {
            var app = CreateApplication(Definition, context);

            app.OnExecute(() =>
            {
                context.Response.AddStatement(new ClearTerminal());
                context.Response.AddStatement(new ClearTerminalHistory());

                return Exit();
            });

            Execute(app, context);
        }

    }
}
