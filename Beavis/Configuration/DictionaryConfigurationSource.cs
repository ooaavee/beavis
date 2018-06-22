using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Beavis.Configuration
{
    public sealed class DictionaryConfigurationSource : IConfigurationSource
    {
        private readonly IDictionary<string, string> _data;

        public DictionaryConfigurationSource(IDictionary<string, string> data)
        {
            _data = data;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DictionaryConfigurationProvider(_data);
        }
    }
}