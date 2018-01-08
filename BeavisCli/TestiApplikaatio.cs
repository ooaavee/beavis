using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    public class TestiApplikaatio : AbstractApplication
    {
        public override ApplicationInfo GetInfo()
        {
            throw new NotImplementedException();
        }

        public override async Task ExecuteAsync(ICommandLineApplication app, CliContext context)
        {
            ICommandOption opt1 = app.Option("-opt1", "Description", CommandOptionType.SingleValue);

            await base.OnExecuteAsync(() =>
            {
                string sss = null;

                return ExitWithHelp(app);
                //return Exit();

            }, app, context);

        }
    }
}