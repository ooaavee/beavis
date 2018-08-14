using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("license", "Displays license information.")]
    public class License : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(async () =>
            {
                const string licenseFileEmbeddedResource = "BeavisCli.Resources.license.all.txt";

                string text;

                using (Stream stream = Assembly.GetAssembly(typeof(License)).GetManifestResourceStream(licenseFileEmbeddedResource))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        text = await reader.ReadToEndAsync();
                    }
                }

                context.WriteText(text);

                return await context.ExitAsync();
            });
        }
    }
}