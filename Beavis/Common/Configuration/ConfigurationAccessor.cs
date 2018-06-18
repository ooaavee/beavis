using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace Beavis.Configuration
{
    /// <summary>
    /// A service for accessing application's configuration.
    /// </summary>
    public class ConfigurationAccessor
    {
        private readonly IConfiguration _configuration;

        public ConfigurationAccessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// The configuration key value pairs for the application
        /// </summary>
        public Dictionary<string, string> GetConfiguration()
        {
            var data = new Dictionary<string, string>();

            IDictionary<string, string> GetData(object provider)
            {
                var type = provider.GetType();
                var property = type.GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic);
                if (property != null)
                {
                    return property.GetValue(provider) as IDictionary<string, string>;

                }
                return null;
            }

            var root = (ConfigurationRoot)_configuration;

            foreach (var provider in root.Providers)
            {
                var d = GetData(provider);
                if (d != null)
                {
                    foreach (var key in d.Keys)
                    {
                        data[key] = d[key];
                    }
                }
            }

            return data;
        }
    }
}
