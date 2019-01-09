using BeavisCli.Commands;
using BeavisCli.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public sealed class BeavisCliOptions
    {
        public BeavisCliOptions()
        {
            BuiltInCommands = GetBuiltInCommands();
        }

        public string Path { get; set; } = "/terminal";

        public bool DisplayExceptions { get; set; } = true;

        public string Prompt { get; set; } = "> ";

        public string Title { get; set; } = "Beavis CLI";

        /// <summary>
        /// IUnauthorizedHandler
        /// </summary>
        public ServiceDefinition UnauthorizedHandlerService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(UnauthorizedHandler),
            ServiceType = typeof(IUnauthorizedHandler)
        };

        /// <summary>
        /// ITerminalInitializer
        /// </summary>
        public ServiceDefinition TerminalInitializerService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(TerminalInitializer),
            ServiceType = typeof(ITerminalInitializer)
        };

        /// <summary>
        /// IFileStorage
        /// </summary>
        public ServiceDefinition FileStorageService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(FileStorage),
            ServiceType = typeof(IFileStorage)
        };

        /// <summary>
        /// ICommandExecutionEnvironment
        /// </summary>
        public ServiceDefinition CommandExecutionEnvironmentService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(CommandExecutionEnvironment),
            ServiceType = typeof(ICommandExecutionEnvironment)
        };

        /// <summary>
        /// ICommandProvider
        /// </summary>
        public ServiceDefinition CommandProviderService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(CommandProvider),
            ServiceType = typeof(ICommandProvider)
        };

        /// <summary>
        /// IRequestHandler
        /// </summary>
        public ServiceDefinition RequestHandlerService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(RequestHandler),
            ServiceType = typeof(IRequestHandler)
        };

        /// <summary>
        /// IJobPool
        /// </summary>
        public ServiceDefinition JobPoolService { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(JobPool),
            ServiceType = typeof(IJobPool)
        };

        /// <summary>
        /// Define built-in command behaviours.
        /// </summary>
        public IReadOnlyDictionary<string, CommandDefinition> BuiltInCommands { get; }

        public Func<BeavisCliRequestTypes, HttpContext, bool> IsRequestTypeBlocked { get; set; }

        private static IReadOnlyDictionary<string, CommandDefinition> GetBuiltInCommands()
        {
            var values = new Dictionary<string, CommandDefinition>();

            void Add(Type type)
            {
                values[CommandInfo.Get(type).Name] = new CommandDefinition(type);
            }

            Add(typeof(Help));
            Add(typeof(Clear));
            Add(typeof(Reset));
            Add(typeof(Shortcuts));
            Add(typeof(License));
            Add(typeof(Upload));
            Add(typeof(Files));

            return values;
        }
    }
}