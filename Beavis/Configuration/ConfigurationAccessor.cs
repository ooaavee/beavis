using Microsoft.Extensions.Configuration;
using System;
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
        public Dictionary<string, string> GetData()
        {
            var data = new Dictionary<string, string>();

            IDictionary<string, string> GetData(object provider)
            {
                Type type = provider.GetType();
                PropertyInfo property = type.GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic);
                if (property != null)
                {
                    return property.GetValue(provider) as IDictionary<string, string>;

                }
                return null;
            }

            ConfigurationRoot root = (ConfigurationRoot)_configuration;

            foreach (IConfigurationProvider provider in root.Providers)
            {
                IDictionary<string, string> subData = GetData(provider);
                if (subData != null)
                {
                    foreach (string key in subData.Keys)
                    {
                        data[key] = subData[key];
                    }
                }
            }

            return data;
        }
    }
}
