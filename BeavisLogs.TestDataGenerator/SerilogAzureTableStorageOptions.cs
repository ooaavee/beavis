using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeavisLogs.TestDataGenerator
{
    public class SerilogAzureTableStorageOptions
    {
        public Sink[] Sinks { get; set; }

        public class Sink
        {
            public string TableName { get; set; }

            public string ConnectionString { get; set; }
        }
    }

    
}
