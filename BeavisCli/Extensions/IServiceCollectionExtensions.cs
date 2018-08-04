using BeavisCli;
using BeavisCli.Services;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        internal static bool ConfigureServicesFlag;

        public static IServiceCollection AddBeavisCli(this IServiceCollection services)
        {
            return services.AddBeavisCli(options => { });
        }

        public static IServiceCollection AddBeavisCli(this IServiceCollection services, Action<BeavisCliOptions> setupAction)
        {
            var options = new BeavisCliOptions();
            setupAction(options);
            services.Configure(setupAction);

            services.Add(ServiceDescriptor.Describe(
                options.CommandProviderService.ServiceType,
                options.CommandProviderService.ImplementationType,
                options.CommandProviderService.Lifetime));

            services.Add(ServiceDescriptor.Describe(
                options.RequestExecutorService.ServiceType,
                options.RequestExecutorService.ImplementationType,
                options.RequestExecutorService.Lifetime));

            services.Add(ServiceDescriptor.Describe(
                options.JobPoolService.ServiceType,
                options.JobPoolService.ImplementationType,
                options.JobPoolService.Lifetime));

            services.Add(ServiceDescriptor.Describe(
                options.UnauthorizedHandlerService.ServiceType,
                options.UnauthorizedHandlerService.ImplementationType,
                options.UnauthorizedHandlerService.Lifetime));

            services.Add(ServiceDescriptor.Describe(
                options.TerminalInitializerService.ServiceType,
                options.TerminalInitializerService.ImplementationType,
                options.TerminalInitializerService.Lifetime));

            services.Add(ServiceDescriptor.Describe(
                options.FileStorageService.ServiceType,
                options.FileStorageService.ImplementationType,
                options.FileStorageService.Lifetime));

            services.Add(ServiceDescriptor.Describe(
                options.AuthorizationHandlerService.ServiceType,
                options.AuthorizationHandlerService.ImplementationType,
                options.AuthorizationHandlerService.Lifetime));

            // default commands
            foreach (CommandDefinition definition in options.BuiltInCommands.Values)
            {
                if (definition.IsEnabled)
                {
                    services.Add(ServiceDescriptor.Singleton(typeof(Command), definition.ImplementationType));
                }
            }

            ConfigureServicesFlag = true;

            return services;
        }

        public static IServiceCollection AddScopedCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : Command
        {
            ValidateCommand<TWebCliCommand>();
            return services.AddScoped<Command, TWebCliCommand>();
        }

        public static IServiceCollection AddSingletonCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : Command
        {
            ValidateCommand<TWebCliCommand>();
            return services.AddSingleton<Command, TWebCliCommand>();
        }

        public static IServiceCollection AddTransientCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : Command
        {
            ValidateCommand<TWebCliCommand>();
            return services.AddTransient<Command, TWebCliCommand>();
        }

        private static void ValidateCommand<TWebCliCommand>() where TWebCliCommand : Command
        {
            CommandInfo info = CommandInfo.ForType(typeof(TWebCliCommand));

            if (info == null)
            {
                throw new InvalidOperationException($"{typeof(TWebCliCommand)} is not a valid command, unable to find {nameof(CommandAttribute)} attribute.");
            }

            if (string.IsNullOrEmpty(info.Name))
            {
                throw new InvalidOperationException($"{nameof(CommandAttribute)}.{nameof(CommandAttribute.Name)} is mandatory.");
            }

            if (string.IsNullOrEmpty(info.FullName))
            {
                throw new InvalidOperationException($"{nameof(CommandAttribute)}.{nameof(CommandAttribute.FullName)} is mandatory.");
            }
        }
    }
}
