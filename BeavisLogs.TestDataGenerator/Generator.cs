﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BeavisLogs.TestDataGenerator
{
    public class Generator
    {
        private readonly SerilogAzureTableStorageOptions _options;

        public Generator(IOptions<SerilogAzureTableStorageOptions> options)
        {
            _options = options.Value;
        }


    }
}
