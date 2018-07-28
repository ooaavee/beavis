using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [WebCliCommand("license", "License information")]
    public class License : WebCliCommand
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(async () =>
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

                context.Response.WriteInformation(text);

                return await ExitAsync(context);
            }, context);
        }
    }
}