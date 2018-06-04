using Beavis.Isolation.Contracts;
using JKang.IpcServiceFramework;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beavis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (ModuleStartupContract.TryParse(args, out ModuleStartupContract contract))
            {
                Startup startup = new Startup(contract);
                IServiceCollection services = new ServiceCollection();
                startup.ConfigureServices(services);
                ServiceProvider serviceProvider = services.BuildServiceProvider();

                IpcServiceHostBuilder
                    .Buid(contract.PipeName, serviceProvider)
                    .Run();
            }
            else
            {
                WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup> ()
                    .Build()
                    .Run();
            }
        }
    }
}
