using BeavisCli.Commands;
using BeavisCli.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace BeavisCli
{
    public sealed class BeavisCliOptions
    {
        public BeavisCliOptions()
        {
            BuiltInCommands = GetBuiltInCommands();
        }

        public string Path { get; set; } = "/beaviscli";

        public bool DisplayExceptions { get; set; }

        public ServiceDefinition UnauthorizedHandlerService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(DefaultUnauthorizedHandler),
            ServiceType = typeof(IUnauthorizedHandler)
        };

        public ServiceDefinition TerminalBehaviourService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(DefaultTerminalBehaviour),
            ServiceType = typeof(ITerminalBehaviour)
        };

        public ServiceDefinition FileStorageService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(DefaultFileStorage),
            ServiceType = typeof(IFileStorage)
        };

        public ServiceDefinition AuthorizationHandlerService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(DefaultAuthorizationHandler),
            ServiceType = typeof(IAuthorizationHandler)
        };

        public ServiceDefinition CommandProviderService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(DefaultCommandProvider),
            ServiceType = typeof(ICommandProvider)
        };

        public ServiceDefinition RequestExecutorService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(DefaultRequestExecutor),
            ServiceType = typeof(IRequestExecutor)
        };

        public ServiceDefinition JobPoolService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(DefaultJobPool),
            ServiceType = typeof(IJobPool)
        };

        public IReadOnlyDictionary<string, CommandDefinition> BuiltInCommands { get; }

        private static IReadOnlyDictionary<string, CommandDefinition> GetBuiltInCommands()
        {
            var values = new Dictionary<string, CommandDefinition>();

            void Init<TWebCliCommand>() where TWebCliCommand : Command
            {
                var info = CommandInfo.ForType(typeof(TWebCliCommand));
                var definition = new CommandDefinition { ImplementationType = typeof(TWebCliCommand) };
                values[info.Name] = definition;
            }

            Init<Help>();
            Init<Clear>();
            Init<Reset>();
            Init<Shortcuts>();
            Init<License>();
            Init<Upload>();
            Init<FileStorage>();

            return values;
        }
    }
}