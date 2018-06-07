using Beavis.Isolation.Contracts;
using Beavis.Isolation.Modules;
using JKang.IpcServiceFramework;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Beavis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (ModuleRuntimeContract.TryParse(args, out var contract))
            {
                IpcServiceHostBuilder
                    .Buid(contract.PipeName, ModuleInitializer.Initialize(contract))
                    .Run();
            }
            else
            {
                WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
            }
        }
    }
}
