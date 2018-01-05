using BeavisCli;
using System.Threading.Tasks;
using BeavisCli.ClientSideStatements;

namespace Jemma.Terminal.Applications.Logout
{
    internal class Logout : AbstractBeavisApplication
    {
        public static readonly BeavisApplicationInfo Definition = new BeavisApplicationInfo
        {
            ////Type = typeof(Logout),
            Name = "logout",
            Description = "Logs out from the server.",
            AllowAnonymous = true
        };


        protected override async Task OnRunAsync(TerminalExecutionContext context)
        {
            var app = CreateApplication(Definition, context);

            app.OnExecute(() =>
            {
                context.Response.AddStatement(new ClearTerminalStatement());
                context.Response.AddStatement(new ClearTerminalHistoryStatement());

                return Exit();
            });

            Execute(app, context);
        }

    }
}
