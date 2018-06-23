using System;
using Beavis.Ipc;
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
                Startup.Module.Options = options;

                new WebHostBuilder()
                    .UseStartup<Startup.Module>()
                    .UseBeavisServer(o =>
                    {                       
                        o.PipeName = options.PipeName;
                        o.ReturnStackTrace = true;
                    })
                    .Build()
                    .Run();
            }
            else
            {
                WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup.Host>()
                    .Build()
                    .Run();
            }
        }
    }
}
