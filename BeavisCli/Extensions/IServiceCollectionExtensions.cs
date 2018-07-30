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

            services.AddSingleton<ICommandProvider, CommandProvider>();
            services.AddSingleton<IRequestExecutor, RequestExecutor>();
            services.AddSingleton<IJobPool, JobPool>();

            // IUnauthorizedHandler
            services.Add(options.UnauthorizedHandlerType == null
                ? ServiceDescriptor.Singleton(typeof(IUnauthorizedHandler), typeof(UnauthorizedHandler))
                : ServiceDescriptor.Singleton(typeof(IUnauthorizedHandler), options.UnauthorizedHandlerType));

            // ITerminalBehaviour
            services.Add(options.TerminalBehaviourType == null
                ? ServiceDescriptor.Singleton(typeof(ITerminalBehaviour), typeof(TerminalBehaviour))
                : ServiceDescriptor.Singleton(typeof(ITerminalBehaviour), options.TerminalBehaviourType));

            // IFileStorage
            services.Add(options.FileStorageType == null
                ? ServiceDescriptor.Singleton(typeof(IFileStorage), typeof(FileStorage))
                : ServiceDescriptor.Singleton(typeof(IFileStorage), options.FileStorageType));

            // IAuthorizationHandler
            services.Add(options.AuthorizationHandlerType == null
                ? ServiceDescriptor.Singleton(typeof(IAuthorizationHandler), typeof(AuthorizationHandler))
                : ServiceDescriptor.Singleton(typeof(IAuthorizationHandler), options.AuthorizationHandlerType));

            // register default commands
            foreach (CommandDefinition definition in options.BuiltInCommands.Values)
            {
                if (definition.IsEnabled)
                {
                    services.Add(ServiceDescriptor.Singleton(typeof(Command), definition.Type));
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
            CommandInfo info = CommandInfo.FromType(typeof(TWebCliCommand));

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
