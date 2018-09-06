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
