using System.Collections.Generic;

namespace BeavisLogs.Models.DataSources
{
    public class DriverProperties
    {
        /// <summary>
        /// A key/value collection of driver properties
        /// </summary>
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}
