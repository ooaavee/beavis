using BeavisCli.Resources;
using System.Threading.Tasks;

namespace BeavisCli.BuiltInCommands
{
    [Command("license", "Displays license information.")]
    public class License : ICommand
    {
        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            await context.OnExecuteAsync(async () =>
            {
                string s = await ResourceReader.GetLicenseAsync();
                context.WriteText(s);
                return await context.ExitAsync();
            });
        }
    }
}