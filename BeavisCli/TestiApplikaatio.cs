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

        public override async Task ExecuteAsync(ApplicationExecutionContext context)
        {
            ICommandOption opt1 = context.Host.Option("-opt1", "Description", CommandOptionType.SingleValue);
          
            await base.OnExecuteAsync(() =>
            {
                string sss = null;

                return ExitWithHelp(context);
                //return Exit();

            }, context);

        }
    }
}