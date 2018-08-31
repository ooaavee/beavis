using System.Collections.Generic;

namespace BeavisLogs.Models.DataSources
{
    public class DataSourceProperties
    {
        /// <summary>
        /// A key/value collection of data source properties
        /// </summary>
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}
