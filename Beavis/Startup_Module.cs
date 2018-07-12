using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Beavis.Configuration;
using Beavis.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.WsFederation;

namespace Beavis
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Startup_Module
    {
        private ILogger<Startup_Module> _logger;

        private readonly ModuleLoader _module;

        public Startup_Module()
        {
            _module = new ModuleLoader(
                ModuleHostingContext.Current.Options.GetConfiguration(),
                ModuleHostingContext.Current.Options.BaseDirectory,
                ModuleHostingContext.Current.Options.AssemblyFileName);
        }

        /// <summary>
        /// Configure services; first configure common services for all modules
        /// and then module specific services.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: Ota tässä fiksumpi lokitus käyttöön!
            services.AddLogging(x => { x.AddConsole(); });

            services.AddSingleton(new ConfigurationAccessor(ModuleHostingContext.Current.Options.GetConfiguration()));

            _module.ConfigureServices(services);
        }


        /// <summary>
        /// Configure HTTP pipeline for the module.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _module.UseServices(app.ApplicationServices);


            ILoggerFactory loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Startup_Module>();




            _logger.LogCritical("dudiin");

            _module.Configure(app, env);

            //Type t = _moduleAssembly.GetType("Demo1.Services.ValueService");

            //dynamic ins = Activator.CreateInstance(t);

            //var values = ins.Values();


        }

       


        

       

        
        

    }
}
