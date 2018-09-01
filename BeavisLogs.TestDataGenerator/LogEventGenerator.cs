using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BeavisLogs.TestDataGenerator
{
    public class LogEventGenerator
    {
        private readonly SerilogAzureTableStorageOptions _options;

        public LogEventGenerator(IOptions<SerilogAzureTableStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task GenerateLogEventsAsync()
        {

        }
    }
}
