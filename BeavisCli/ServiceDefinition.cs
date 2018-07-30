using System;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli
{
    public struct ServiceDefinition
    {
        public ServiceLifetime Lifetime { get; set; }

        public Type ImplementationType { get; set; }

        internal Type ServiceType { get; set; }
    }
}