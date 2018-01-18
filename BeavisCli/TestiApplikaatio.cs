using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    public class TestiApplikaatio : WebCliApplication
    {
        public TestiApplikaatio() : base("testi", "description...") { }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            IOption opt1 = context.Option("-opt1", "Description", CommandOptionType.SingleValue);

            await base.OnExecuteAsync(() =>
            {
                string sss = null;

                return ExitWithHelp(context);
                //return Exit();

            }, context);

        }
    }
}