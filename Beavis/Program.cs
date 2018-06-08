using System;
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
            if (ModuleRuntimeContract.TryParse(args, out ModuleRuntimeContract contract))
            {
                StartModule(contract);
            }
            else
            {
                StartHost();
            }
        }

        private static void StartHost()
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        private static void StartModule(ModuleRuntimeContract contract)
        {
            IServiceProvider serviceProvider;

            try
            {
                serviceProvider = ModuleInitializer.Initialize(contract);
            }
            catch (ModuleInitializerException exception)
            {
                return;
            }


            IpcServiceHostBuilder
                .Buid(contract.PipeName, serviceProvider)
                .Run();
        }
    }


}
