using System;
using System.Threading.Tasks;
using BeavisCli;
using BeavisCli.Internal;
using BeavisCli.Internal.Applications;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddWebCli(this IServiceCollection services, Action<WebCliOptions> setupAction = null)
        {
            var options = new WebCliOptions();

            if (setupAction == null)
            {
                setupAction = o => { };
            }

            setupAction(options);

            services.Configure(setupAction);

            services.AddSingleton<WebCliSandbox>();
            services.AddSingleton<IJobPool, DefaultJobPool>();
            services.AddSingleton<DefaultJobPool>();

            services.Add(options.UnauthorizedHandlerType == null
                ? ServiceDescriptor.Singleton(typeof(IUnauthorizedHandler), typeof(NullVoidService))
                : ServiceDescriptor.Singleton(typeof(IUnauthorizedHandler), options.UnauthorizedHandlerType));

            services.Add(options.TerminalInitializerType == null
                ? ServiceDescriptor.Singleton(typeof(IWebCliInitializer), typeof(NullVoidService))
                : ServiceDescriptor.Singleton(typeof(IWebCliInitializer), options.TerminalInitializerType));

            services.Add(options.FileUploadStorageType == null
                ? ServiceDescriptor.Singleton(typeof(IFileUploadStorage), typeof(NullVoidService))
                : ServiceDescriptor.Singleton(typeof(IFileUploadStorage), options.FileUploadStorageType));

            if (options.EnableDefaultApplications)
            {
                services.AddSingletonWebCliApplication<Help>();
                services.AddSingletonWebCliApplication<Clear>();
                services.AddSingletonWebCliApplication<Reset>();
                services.AddSingletonWebCliApplication<Shortcuts>();

                if (options.EnableFileUpload)
                {
                    if (options.FileUploadStorageType == null)
                    {
                        throw new InvalidOperationException($"{nameof(options.EnableFileUpload)} is true, but {nameof(options.FileUploadStorageType)} has not been set.");
                    }

                    services.AddSingletonWebCliApplication<Upload>();
                }
            }

            services.AddMemoryCache();

            return services;
        }

        public static IServiceCollection AddScopedWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            return services.AddScoped<WebCliApplication, TWebCliApplication>();
        }

        public static IServiceCollection AddSingletonWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            return services.AddSingleton<WebCliApplication, TWebCliApplication>();
        }

        public static IServiceCollection AddTransientWebCliApplication<TWebCliApplication>(this IServiceCollection services) where TWebCliApplication : WebCliApplication
        {
            return services.AddTransient<WebCliApplication, TWebCliApplication>();
        }


        private class NullVoidService : IUnauthorizedHandler, IWebCliInitializer, IFileUploadStorage
        {
            void IUnauthorizedHandler.OnUnauthorized(WebCliContext context)
            {
            }

            void IWebCliInitializer.Initialize(HttpContext context, WebCliResponse response)
            {
            }

            Task IFileUploadStorage.UploadAsync(UploadedFile file, WebCliResponse response)
            {
                return Task.CompletedTask;
            }
        }
    }
}
