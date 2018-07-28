using BeavisCli;
using BeavisCli.Internal;
using BeavisCli.Services;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        internal static bool ConfigureServicesFlag;

        public static IServiceCollection AddWebCli(this IServiceCollection services)
        {
            return services.AddWebCli(options => { });
        }

        public static IServiceCollection AddWebCli(this IServiceCollection services, Action<WebCliOptions> setupAction)
        {
            var options = new WebCliOptions();
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
                    services.Add(ServiceDescriptor.Singleton(typeof(WebCliCommand), definition.Type));
                }
            }

            ConfigureServicesFlag = true;

            return services;
        }

        public static IServiceCollection AddScopedWebCliCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : WebCliCommand
        {
            ValidateCommand<TWebCliCommand>();
            return services.AddScoped<WebCliCommand, TWebCliCommand>();
        }

        public static IServiceCollection AddSingletonWebCliCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : WebCliCommand
        {
            ValidateCommand<TWebCliCommand>();
            return services.AddSingleton<WebCliCommand, TWebCliCommand>();
        }
      
        public static IServiceCollection AddTransientWebCliCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : WebCliCommand
        {
            ValidateCommand<TWebCliCommand>();
            return services.AddTransient<WebCliCommand, TWebCliCommand>();
        }

        private static void ValidateCommand<TWebCliCommand>() where TWebCliCommand : WebCliCommand
        {
            WebCliCommandInfo info = WebCliCommandInfo.FromType(typeof(TWebCliCommand));

            if (info == null)
            {
                throw new InvalidOperationException($"{typeof(TWebCliCommand)} is not a valid command, unable to find {nameof(WebCliCommandAttribute)} attribute.");
            }

            if (string.IsNullOrEmpty(info.Name))
            {
                throw new InvalidOperationException($"{nameof(WebCliCommandAttribute)}.{nameof(WebCliCommandAttribute.Name)} is mandatory.");
            }

            if (string.IsNullOrEmpty(info.FullName))
            {
                throw new InvalidOperationException($"{nameof(WebCliCommandAttribute)}.{nameof(WebCliCommandAttribute.FullName)} is mandatory.");
            }
        }
    }
}
