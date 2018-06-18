using System;
using Beavis.Isolation.Contracts;
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
                //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Startup.ForModule.Options = options;

                new WebHostBuilder()
                    .UseStartup<Startup.ForModule>()
                    .Build()
                    .Run();
            }
            else
            {
                WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup.ForHost>()
                    .Build()
                    .Run();
            }
        }

        //private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    string path = @"d:\home\starttivirhe.txt";
        //    string text = e.ExceptionObject.ToString();
        //   System.IO.File.WriteAllText(path, text);

        //}

        //private static void StartModule(ModuleStartupOptions contract)
        //{
        //    IServiceProvider serviceProvider;
        //    try
        //    {
        //        serviceProvider = ModuleInitializer.Initialize(contract);
        //    }
        //    catch (ModuleInitializerException exception)
        //    {
        //        return;
        //    }


        //    IpcServiceHostBuilder
        //        .Buid(contract.PipeName, serviceProvider)
        //        .Run();
        //}
    }





}
