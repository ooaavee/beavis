using BeavisLogs.Drivers;
using BeavisLogs.Models.DataSources;

namespace BeavisLogs.Services
{
    public class QueryContextFactory
    {
        private readonly LogEventTempStorage _temp;

        public QueryContextFactory(LogEventTempStorage temp)
        {
            _temp = temp;
        }

        public QueryContext CreateContext(DataSource source)
        {
            LogEventSlot slot = _temp.CreateSlot(source.Info);
            QueryContext context = new QueryContext(slot, source.DriverProperties);
            return context;
        }
    }
}
