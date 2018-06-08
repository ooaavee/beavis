using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Beavis.Configuration
{
    public sealed class DictionaryConfigurationProvider : ConfigurationProvider
    {
        private readonly IDictionary<string, string> _data;

        public DictionaryConfigurationProvider(IDictionary<string, string> data)
        {
            _data = data;
        }

        public override void Load()
        {
            Data = _data;
        }
    }
}