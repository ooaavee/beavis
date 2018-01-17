using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    public class TestiApplikaatio : WebCliApplication
    {
        public TestiApplikaatio() : base("testi", "description...") { }

        public override async Task ExecuteAsync(WebCliContext context)
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