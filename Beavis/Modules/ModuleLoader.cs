using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Logging;

namespace Beavis.Modules
{
    public class ModuleLoader
    {
        private readonly IConfiguration _configuration;

        private readonly string _baseDirectory;
        private readonly string _moduleAssemblyPath;

        private Assembly _moduleAssembly;
        private ICompilationAssemblyResolver _assemblyResolver;
        private DependencyContext _dependencyContext;
        private AssemblyLoadContext _loadContext;

        private Action<IServiceCollection> _moduleConfigureServices;
        private Action<IApplicationBuilder, IHostingEnvironment> _moduleConfigure;

        public ModuleLoader(IConfiguration configuration, string baseDirectory, string moduleAssemblyFileName)
        {
            _configuration = configuration;
            _baseDirectory = baseDirectory;
            _moduleAssemblyPath = GetFullPath(moduleAssemblyFileName);
                      
            LoadAssemblies();
            ResolveStartup();
        }

        private void LoadAssemblies()
        {
            try
            {
                _moduleAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(_moduleAssemblyPath);
                _dependencyContext = DependencyContext.Load(_moduleAssembly);

                _assemblyResolver = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
                {
                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(_moduleAssemblyPath)),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                });

                _loadContext = AssemblyLoadContext.GetLoadContext(_moduleAssembly);
                _loadContext.Resolving += ModuleAssembly_OnResolving;
            }
            catch (Exception e)
            {
                throw new ModuleLoaderException($"An exception occurred while loading module assembly '{_moduleAssemblyPath}'.", e);
            }
        }

        private void ResolveStartup()
        {
            

            //
            // find Startup type
            //
            var withDefaultConstructor = new List<Type>();
            var withConfigurationConstructor = new List<Type>();

            foreach (Type type in _moduleAssembly.GetExportedTypes().Where(type => type.Name == "Startup" && type.IsClass && type.IsPublic && !type.IsAbstract))
            {
                foreach (ConstructorInfo constructor in type.GetConstructors().Where(constructor => constructor.IsPublic))
                {
                    if (constructor.GetParameters().Length == 1 && constructor.GetParameters()[0].ParameterType == typeof(IConfiguration))
                    {
                        withConfigurationConstructor.Add(type);
                        break;
                    }
                    if (constructor.GetParameters().Length == 0)
                    {
                        withDefaultConstructor.Add(type);
                        break;
                    }
                }
            }

            if (withDefaultConstructor.Count == 0 && withConfigurationConstructor.Count == 0)
            {
                throw new ModuleLoaderException($"Could not find the Startup type from module assembly '{_moduleAssemblyPath}'.");
            }

            if ((withDefaultConstructor.Count > 0 && withConfigurationConstructor.Count > 0) || withDefaultConstructor.Count > 1 || withConfigurationConstructor.Count > 1)
            {
                throw new ModuleLoaderException($"Found more than one suitable Startup type from module assembly '{_moduleAssemblyPath}'.");
            }

            bool startupWithConfiguration = withConfigurationConstructor.Count == 1;
            Type startupType = withConfigurationConstructor.Count == 1 ? withConfigurationConstructor.First() : withDefaultConstructor.First();
            object startup;

            try
            {
                if (startupWithConfiguration)
                {
                    startup = Activator.CreateInstance(startupType, _configuration);
                }
                else
                {
                    startup = Activator.CreateInstance(startupType);
                }
            }
            catch (Exception e)
            {
                throw new ModuleLoaderException("", e);
            }

            //         public void ConfigureServices(IServiceCollection services)
            //         public void Configure(IApplicationBuilder app, IHostingEnvironment env)

        }

        private Assembly ModuleAssembly_OnResolving(AssemblyLoadContext context, AssemblyName name)
        {
            bool NamesMatch(RuntimeLibrary runtime)
            {
                return string.Equals(runtime.Name, name.Name, StringComparison.OrdinalIgnoreCase);
            }

            RuntimeLibrary library = _dependencyContext.RuntimeLibraries.FirstOrDefault(NamesMatch);
            if (library != null)
            {
                var wrapper = new CompilationLibrary(
                    library.Type,
                    library.Name,
                    library.Version,
                    library.Hash,
                    library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                    library.Dependencies,
                    library.Serviceable);

                var assemblies = new List<string>();
                _assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
                if (assemblies.Count > 0)
                {
                    return _loadContext.LoadFromAssemblyPath(assemblies[0]);
                }
            }

            string path = GetFullPath($"{name.Name}.dll");

            Assembly resolvedAssembly = _loadContext.LoadFromAssemblyPath(path);

            return resolvedAssembly;
        }

        private string GetFullPath(string fileName)
        {
            string path = Path.Combine(_baseDirectory, "app", fileName);
            return path;
        }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

        }

        public void UseServices(IServiceProvider serviceProvider)
        {
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            ILogger<ModuleLoader> logger = loggerFactory.CreateLogger<ModuleLoader>();
            ((TempLogger)Logger).Flush(logger);
            Logger = logger;
        }

        private ILogger Logger { get; set; } = new TempLogger();

        private const string Disclaimer = "";

        private string FormatLogMessage(string message)
        {
            if (Logger is TempLogger)
            {
                // TODO: Lisää tässä messagen loppuun joku pätkä, joka kertoo, että viesti on cachettu ja flushattu vasta nyt koska tapahtumahetkellä loggeria ei ole käytössä.
            }
            else
            {
                
            }

            return message;
        }

        private class TempLogger : ILogger
        {
            private readonly  IList<ILogEvent> _pending = new List<ILogEvent>();

            public void Flush(ILogger logger)
            {
                foreach (ILogEvent evt in _pending)
                {
                    evt.Flush(logger);
                }
            }

            private class LogEvent<TState> : ILogEvent
            {
                public LogLevel LogLevel { private get; set; }
                public EventId EventId { private get; set; }
                public TState State { private get; set; }
                public Exception Exception { private get; set; }
                public Func<TState, Exception, string> Formatter { private get; set; }

                public void Flush(ILogger logger)
                {
                    logger.Log<TState>(LogLevel, EventId, State, Exception, Formatter);
                }
            }

            private interface ILogEvent
            {
                void Flush(ILogger logger);
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _pending.Add(new LogEvent<TState>
                {
                    LogLevel = logLevel,
                    EventId = eventId,
                    State = state,
                    Exception = exception,
                    Formatter = formatter
                });
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }
        }

    }
}
