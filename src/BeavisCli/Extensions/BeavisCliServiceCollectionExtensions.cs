using BeavisCli;
using BeavisCli.Extensions;
using BeavisCli.Services;
using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BeavisCliServiceCollectionExtensions
    {
        internal static bool ServicesRegistered;

        public static IServiceCollection AddBeavisCli(this IServiceCollection services, Action<BeavisCliOptions> setup = null)
        {
            // register options
            var options = new BeavisCliOptions();

            if (setup == null)
            {
                setup = x => { };
            }

            setup(options);
            services.Configure(setup);

            // register required services
            services.Add(options.CommandProvider);
            services.Add(options.RequestHandler);
            services.Add(options.JobPoolService);
            services.Add(options.UnauthorizedHandler);
            services.Add(options.TerminalInitializer);
            services.Add(options.FileStorage);
            services.Add(options.CommandExecutionEnvironment);

            // register built-in commands
            foreach (CommandBehaviour command in options.BuiltInCommandBehaviours.Values)
            {
                if (!command.IsEnabled)
                {
                    continue;
                }

                services.Add(ServiceDescriptor.Singleton(typeof(ICommand), command.ImplementationType));
            }

            ServicesRegistered = true;

            return services;
        }

        public static IServiceCollection AddScopedCommand<TCommand>(this IServiceCollection services) where TCommand : ICommand
        {
            Validate<TCommand>();

            return services.Add(new ServiceDefinition
            {
                ServiceType = typeof(ICommand),
                ImplementationType = typeof(TCommand), Lifetime = ServiceLifetime.Scoped
            });
        }

        public static IServiceCollection AddSingletonCommand<TCommand>(this IServiceCollection services) where TCommand : ICommand
        {
            Validate<TCommand>();

            return services.Add(new ServiceDefinition
            {
                ServiceType = typeof(ICommand),
                ImplementationType = typeof(TCommand),
                Lifetime = ServiceLifetime.Singleton
            });
        }

        public static IServiceCollection AddTransientCommand<TCommand>(this IServiceCollection services) where TCommand : ICommand
        {
            Validate<TCommand>();

            return services.Add(new ServiceDefinition
            {
                ServiceType = typeof(ICommand),
                ImplementationType = typeof(TCommand),
                Lifetime = ServiceLifetime.Transient
            });
        }

        private static IServiceCollection Add(this IServiceCollection services, ServiceDefinition service)
        {
            services.Add(ServiceDescriptor.Describe(service.ServiceType, service.ImplementationType, service.Lifetime));
            return services;
        }

        /// <summary>
        /// Validates that the specified type meets the requirements for a command. 
        /// </summary>
        private static void Validate<TCommand>()
        {
            Type commandType = typeof(TCommand);

            CommandInfo info = commandType.GetCommandInfo();

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

            // CommandAttribute.Description is required
            if (string.IsNullOrEmpty(info.Description))
            {
                throw new InvalidOperationException($"{nameof(CommandAttribute)}.{nameof(CommandAttribute.Description)} is mandatory.");
            }
        }
    }
}
