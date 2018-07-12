using System;
using Beavis.Ipc;
using Beavis.Modules;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace Beavis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ModuleRuntimeOptions options;

            ProgramMode mode = GetMode(args, out options);

            switch (mode)
            {
                case ProgramMode.Host:
                    RunHost();
                    break;

                case ProgramMode.Module:
                    RunModule(options);
                    break;
            }
        }

        private static ProgramMode GetMode(string[] args, out ModuleRuntimeOptions options)
        {
            options = null;

            if (args != null && args.First() == "-moduledev")
            {
                options = new ModuleRuntimeOptions
                {
                    PipeName = "DEV",
                    ModuleKey = "DEV",
                    BaseDirectory = @"C:\Projects\beavis\Beavis\bin\Debug\netcoreapp2.1",
                    AssemblyFileName = "Demo1.dll"
                };
                return ProgramMode.Module;
            }

            if (ModuleRuntimeOptions.TryParse(args, out options))
            {
                return ProgramMode.Module;
            }

            return ProgramMode.Host;
        }

        private static void RunHost()
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup_Host>()               
                .Build()
                .Run();
        }

        private static void RunModule(ModuleRuntimeOptions options)
        {
            ////try
            ////{
            ////    Startup_Module.Initialize(options);
            ////}
            ////catch (Exception e)
            ////{
            ////    var s = new StringBuilder();
            ////    s.AppendLine("An error occurred while initializing module.");
            ////    s.AppendLine("");
            ////    s.AppendLine("Exception:");
            ////    s.AppendLine(e.ToString());
            ////    s.AppendLine("");
            ////    s.AppendLine("Module startup options:");
            ////    s.AppendLine(options.AsSerialized());
            ////    throw;
            ////}
            
            ModuleHostingContext.Build(options);

            new WebHostBuilder()
                .UseStartup<Startup_Module>()
                .UseBeavisServer(o =>
                {
                    o.PipeName = options.PipeName;
                    o.ReturnStackTrace = true;                    
                })
                .Build()
                .Run();
        }

        private enum ProgramMode
        {
            Host,
            Module
        }
    }
}
