using Beavis.Modules;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Beavis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (ModuleStartupOptions.TryParse(args, out var options))
            {
                RunModule(options);
            }
            else
            {
                RunHost();
            }
        }

        private static void RunHost()
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup.Host>()
                .Build()
                .Run();
        }

        private static void RunModule(ModuleStartupOptions options)
        {
            Startup.Module.Options = options;

            new WebHostBuilder()
                .UseStartup<Startup.Module>()
                .UseBeavisServer(o => { o.PipeName = options.PipeName; })
                .Build()
                .Run();
        }

    }
}
