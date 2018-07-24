using System;
using BeavisCli;
using BeavisCli.Internal;
using BeavisCli.Internal.DefaultServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddWebCli(this IServiceCollection services)
        {
            return services.AddWebCli(options => { });
        }

        public static IServiceCollection AddWebCli(this IServiceCollection services, Action<WebCliOptions> setupAction)
        {
            var options = new WebCliOptions();
            setupAction(options);
            services.Configure(setupAction);

            services.AddSingleton<WebCliSandbox>();          
            services.AddSingleton<JobPool>();

            // IUnauthorizedHandler
            services.Add(options.UnauthorizedHandlerType == null
                ? ServiceDescriptor.Singleton(typeof(IUnauthorizedHandler), typeof(DefaultUnauthorizedHandler))
                : ServiceDescriptor.Singleton(typeof(IUnauthorizedHandler), options.UnauthorizedHandlerType));

            // ITerminalInitializer
            services.Add(options.TerminalInitializerType == null
                ? ServiceDescriptor.Singleton(typeof(ITerminalInitializer), typeof(DefaultTerminalInitializer))
                : ServiceDescriptor.Singleton(typeof(ITerminalInitializer), options.TerminalInitializerType));

            // IFileStorage
            services.Add(options.FileStorageType == null
                ? ServiceDescriptor.Singleton(typeof(IFileStorage), typeof(DefaultFileStorage))
                : ServiceDescriptor.Singleton(typeof(IFileStorage), options.FileStorageType));

            // IAuthorizationHandler
            services.Add(options.AuthorizationHandlerType == null
                ? ServiceDescriptor.Singleton(typeof(IAuthorizationHandler), typeof(DefaultAuthorizationHandler))
                : ServiceDescriptor.Singleton(typeof(IAuthorizationHandler), options.AuthorizationHandlerType));

            // register default commands
            foreach (BuiltInCommandBehaviour behaviour in options.BuiltInCommands.Values)
            {
                if (behaviour.Enabled)
                {
                    services.Add(ServiceDescriptor.Singleton(typeof(WebCliCommand), behaviour.Type));
                }
            }

            return services;
        }

        public static IServiceCollection AddScopedWebCliCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : WebCliCommand
        {
            Validate<TWebCliCommand>();
            return services.AddScoped<WebCliCommand, TWebCliCommand>();
        }

        public static IServiceCollection AddSingletonWebCliCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : WebCliCommand
        {
            Validate<TWebCliCommand>();
            return services.AddSingleton<WebCliCommand, TWebCliCommand>();
        }
      
        public static IServiceCollection AddTransientWebCliCommand<TWebCliCommand>(this IServiceCollection services) where TWebCliCommand : WebCliCommand
        {
            Validate<TWebCliCommand>();
            return services.AddTransient<WebCliCommand, TWebCliCommand>();
        }

        private static void Validate<TWebCliCommand>()where TWebCliCommand : WebCliCommand
        {
            WebCliCommandInfo info = WebCliCommandInfo.Parse(typeof(TWebCliCommand));

            if (info == null)
            {
                throw new InvalidOperationException($"{typeof(TWebCliCommand)} is not a valid command, unable to find {nameof(WebCliCommandAttribute)} attribute.");
            }

            if (string.IsNullOrEmpty(info.Name))
            {
                throw new InvalidOperationException($"{nameof(WebCliCommandAttribute)}.{nameof(WebCliCommandInfo.Name)} is mandatory.");
            }

            if (string.IsNullOrEmpty(info.Description))
            {
                throw new InvalidOperationException($"{nameof(WebCliCommandAttribute)}.{nameof(WebCliCommandInfo.Description)} is mandatory.");
            }
        }
    }
}
