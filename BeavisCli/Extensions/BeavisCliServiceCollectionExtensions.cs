using BeavisCli;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BeavisCliServiceCollectionExtensions
    {
        internal static bool Flag;

        private static readonly Type ServiceTypeForCommand = typeof(Command);

        public static IServiceCollection AddBeavisCli(this IServiceCollection services)
        {
            return services.AddBeavisCli(options => { });
        }

        public static IServiceCollection AddBeavisCli(this IServiceCollection services, Action<BeavisCliOptions> setup)
        {
            // register options
            var options = new BeavisCliOptions();
            setup(options);
            services.Configure(setup);

            // register required services
            services.Add(options.CommandProviderService);
            services.Add(options.CommandExecutorService);
            services.Add(options.JobPoolService);
            services.Add(options.UnauthorizedHandlerService);
            services.Add(options.TerminalInitializerService);
            services.Add(options.FileStorageService);
            services.Add(options.CommandExecutionEnvironmentService);

            // register built-in commands
            foreach (CommandDefinition cmd in options.BuiltInCommands.Values.Where(x => x.IsEnabled))
            {
                services.Add(ServiceDescriptor.Singleton(ServiceTypeForCommand, cmd.ImplementationType));
            }

            Flag = true;

            return services;
        }

        public static IServiceCollection AddScopedCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : Command
        {
            Validate(typeof(TWebCliCommand));
            return services.AddScoped<Command, TWebCliCommand>();
        }

        public static IServiceCollection AddSingletonCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : Command
        {
            Validate(typeof(TWebCliCommand));
            return services.AddSingleton<Command, TWebCliCommand>();
        }

        public static IServiceCollection AddTransientCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : Command
        {
            Validate(typeof(TWebCliCommand));
            return services.AddTransient<Command, TWebCliCommand>();
        }

        private static void Add(this IServiceCollection services, ServiceDefinition service)
        {
            services.Add(ServiceDescriptor.Describe(service.ServiceType, service.ImplementationType, service.Lifetime));
        }

        /// <summary>
        /// Validates that the specified type meets the requirements for a command. 
        /// </summary>
        private static void Validate(Type commandType)
        {
            CommandInfo info = CommandInfo.ForType(commandType);

            // CommandAttribute must be found
            if (info == null)
            {
                throw new InvalidOperationException($"{commandType} is not a valid command, unable to find {nameof(CommandAttribute)} attribute.");
            }

            // CommandAttribute.Name is required
            if (string.IsNullOrEmpty(info.Name))
            {
                throw new InvalidOperationException($"{nameof(CommandAttribute)}.{nameof(CommandAttribute.Name)} is mandatory.");
            }

            // CommandAttribute.FullName is required
            if (string.IsNullOrEmpty(info.FullName))
            {
                throw new InvalidOperationException($"{nameof(CommandAttribute)}.{nameof(CommandAttribute.FullName)} is mandatory.");
            }
        }
    }
}
