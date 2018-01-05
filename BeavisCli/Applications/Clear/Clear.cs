using System.Threading.Tasks;
using BeavisCli;

namespace Jemma.Terminal.Applications.Clear
{
    internal class Clear : AbstractBeavisApplication
    {
        public static readonly BeavisApplicationInfo Definition = new BeavisApplicationInfo
        {
            ////Type = typeof(Clear),
            Name = "clear",
            Description = "Clears the terminal.",
            AllowAnonymous = true
        };

        protected override async Task OnRunAsync(TerminalExecutionContext context)
        {
            var app = CreateApplication(Definition, context);
            app.OnExecute(() => Exit());
            Execute(app, context);
        }

    }
}
