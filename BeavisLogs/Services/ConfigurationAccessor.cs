using Microsoft.Extensions.Configuration;

namespace BeavisLogs.Services
{
    public class ConfigurationAccessor
    {
        private readonly IConfiguration _configuration;

        public ConfigurationAccessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string this[string key]
        {
            get
            {
                string value = _configuration[key];
                return value;
            }
        }
    }
}
