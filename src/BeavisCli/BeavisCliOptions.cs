using BeavisCli.Extensions;
using BeavisCli.Services;
using BeavisCli.Services.Defaults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public sealed class BeavisCliOptions
    {
        public string Path { get; set; } = "/terminal";

        public bool DisplayExceptions { get; set; } = true;

        public string Prompt { get; set; } = "> ";

        public string Title { get; set; } = "Beavis CLI";

        /// <summary>
        /// IUnauthorizedHandler
        /// </summary>
        public ServiceDefinition UnauthorizedHandler { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(UnauthorizedHandler),
            ServiceType = typeof(IUnauthorizedHandler)
        };

        /// <summary>
        /// ITerminalInitializer
        /// </summary>
        public ServiceDefinition TerminalInitializer { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(TerminalInitializer),
            ServiceType = typeof(ITerminalInitializer)
        };

        /// <summary>
        /// IFileStorage
        /// </summary>
        public ServiceDefinition FileStorage { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(FileStorage),
            ServiceType = typeof(IFileStorage)
        };

        /// <summary>
        /// ICommandExecutionEnvironment
        /// </summary>
        public ServiceDefinition CommandExecutionEnvironment { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(CommandExecutionEnvironment),
            ServiceType = typeof(ICommandExecutionEnvironment)
        };

        /// <summary>
        /// ICommandProvider
        /// </summary>
        public ServiceDefinition CommandProvider { get; } = new ServiceDefinition
        {
            Lifetime = ServiceLifetime.Singleton,
            ImplementationType = typeof(CommandProvider),
            ServiceType = typeof(ICommandProvider)
        };

        /// <summary>
        /// IRequestHandler
        /// </summary>
        public ServiceDefinition RequestHandler { get; } = new ServiceDefinition
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
        /// Define built-in command behaviors.
        /// </summary>
        public IReadOnlyDictionary<string, CommandBehaviour> BuiltInCommandBehaviours { get; } = typeof(BeavisCliOptions).Assembly.GetCommands();

        /// <summary>
        /// A method that checks if the specified request is allowed.
        /// </summary>
        public Func<BeavisCliRequestTypes, HttpContext, bool> IsRequestApproved { get; set; } = (type, context) =>
        {
            if (type == BeavisCliRequestTypes.None)
            {
                return false;
            }

            return true;
        };
    }
}