using BeavisLogs.Models.DataSources;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class LogEventMappingContext
    {
        public DriverProperties DriverProperties { get; }

        public LogEventMappingContext(DriverProperties properties)
        {
            DriverProperties = properties;
        }
    }
}