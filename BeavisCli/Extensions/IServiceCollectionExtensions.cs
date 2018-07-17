using System;
using BeavisCli;
using BeavisCli.Internal;
using BeavisCli.Internal.Applications;
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

            // IUploadStorage
            services.Add(options.FileUploadStorageType == null
                ? ServiceDescriptor.Singleton(typeof(IUploadStorage), typeof(DefaultUploadStorage))
                : ServiceDescriptor.Singleton(typeof(IUploadStorage), options.FileUploadStorageType));

            // IAuthorizationHandler
            services.Add(options.AuthorizationHandlerType == null
                ? ServiceDescriptor.Singleton(typeof(IAuthorizationHandler), typeof(DefaultAuthorizationHandler))
                : ServiceDescriptor.Singleton(typeof(IAuthorizationHandler), options.AuthorizationHandlerType));

            // register default applications
            if (options.EnableDefaultApplications)
            {
                services.AddSingletonWebCliApplication<Help>();
                services.AddSingletonWebCliApplication<Clear>();
                services.AddSingletonWebCliApplication<Reset>();
                services.AddSingletonWebCliApplication<Shortcuts>();
                services.AddSingletonWebCliApplication<License>();

                if (options.EnableFileUpload)
                {
                    if (options.FileUploadStorageType == null)
                    {
                        throw new InvalidOperationException($"{nameof(options.EnableFileUpload)} is true, but {nameof(options.FileUploadStorageType)} has not been set.");
                    }

                    services.AddSingletonWebCliApplication<Upload>();
                }
            }

            return services;
        }

        public static IServiceCollection AddScopedWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            Validate<TWebCliApplication>();
            return services.AddScoped<WebCliApplication, TWebCliApplication>();
        }

        public static IServiceCollection AddSingletonWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            Validate<TWebCliApplication>();
            return services.AddSingleton<WebCliApplication, TWebCliApplication>();
        }
      
        public static IServiceCollection AddTransientWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            Validate<TWebCliApplication>();
            return services.AddTransient<WebCliApplication, TWebCliApplication>();
        }

        private static void Validate<TWebCliApplication>()where TWebCliApplication : WebCliApplication
        {
            WebCliApplicationInfo info = WebCliApplicationInfo.Parse(typeof(TWebCliApplication));

            if (info == null)
            {
                throw new InvalidOperationException($"{typeof(TWebCliApplication)} is not a valid WebCliApplication, unable to find {nameof(WebCliApplicationAttribute)} attribute.");
            }

            if (string.IsNullOrEmpty(info.Name))
            {
                throw new InvalidOperationException($"{nameof(WebCliApplicationAttribute)}.{nameof(WebCliApplicationInfo.Name)} is mandatory.");
            }

            if (string.IsNullOrEmpty(info.Description))
            {
                throw new InvalidOperationException($"{nameof(WebCliApplicationAttribute)}.{nameof(WebCliApplicationInfo.Description)} is mandatory.");
            }
        }
    }
}
