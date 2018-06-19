//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Beavis.Isolation.Contracts;
//using Beavis.Middlewares;
//using Beavis.Modules;
//using JKang.IpcServiceFramework;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;

//namespace Beavis.Isolation.Modules
//{
//    public class ModuleInitializer
//    {
//        private ModuleStartupOptions ModuleStartupOptions { get; set; }

//        private ModuleInitializer() { }

//        public static IServiceProvider Initialize(ModuleStartupOptions contract)
//        {
//            var services = new ServiceCollection();
//            var initializer = new ModuleInitializer { ModuleStartupOptions = contract };
//            initializer.LoadAssemblies();
//            initializer.ConfigureServices(services);
//            return services.BuildServiceProvider();
//        }

//        private void LoadAssemblies()
//        {
//            // TODO: Lataa tässä muistiin kaikki assemblyt jotka kuuluvat moduuliin

//            // TODO: Etsi sielä Startup-luokka

//            // TODO: Passaa tämä sielle startup luokalle costructorissaa!
//            IConfiguration configuration = ModuleStartupOptions.GetData();

            
           

//        }

//        /// <summary>
//        /// Configure services
//        /// </summary>
//        private void ConfigureServices(IServiceCollection services)
//        {
//            services
//                .AddLogging(builder =>
//                {
//                    builder.AddConsole();

//                    // TODO: register a customer azure table storage logger here

//                    // TODO: read this from "app settings etc"
//                    builder.SetMinimumLevel(LogLevel.Debug);
//                });


//            services.AddIpc(options => { options.ThreadCount = ModuleStartupOptions.ThreadCount; });

//            // TRANSIENT SERVICES
//            services.AddTransient<IIsolatedModule, IsolatedModule>();

//            // SINGLETON SERVICES
//            var provider = new ModuleRequestHandlerProvider();
//            services.AddSingleton<ModuleRequestHandlerProvider>(x => provider);


//            // TODO: setup logging for this module

//            // TODO: setup custom services for this module

//        }

//    }
//}
