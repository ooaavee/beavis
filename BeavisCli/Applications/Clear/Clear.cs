using System.Threading.Tasks;
using BeavisCli;

namespace Jemma.Terminal.Applications.Clear
{
    internal class Clear : AbstractBeavisApplication
    {
        public static readonly ApplicationInfo Definition = new ApplicationInfo
        {
            ////Type = typeof(Clear),
            Name = "clear",
            Description = "Clears the terminal."
        };

        protected override async Task OnRunAsync(CliContext context)
        {
            var app = CreateApplication(Definition, context);
            app.OnExecute(() => Exit());
            Execute(app, context);
        }

    }
}
