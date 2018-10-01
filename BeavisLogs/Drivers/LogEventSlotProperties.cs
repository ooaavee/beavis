using BeavisLogs.Models.DataSources;

namespace BeavisLogs.Drivers
{
    public class LogEventSlotProperties
    {
        public LogEventOutputRenderer Renderer { get; set; }
        public string Key { get; set; }
        public int Limit { get; set; } = 0;
        public DataSourceInfo[] Sources { get; set; }
    }
}