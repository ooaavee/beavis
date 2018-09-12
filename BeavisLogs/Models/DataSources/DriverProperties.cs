using System.Collections.Generic;

namespace BeavisLogs.Models.DataSources
{
    public sealed class DriverProperties
    {
        /// <summary>
        /// A key/value collection of driver properties
        /// </summary>
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

        public string Get(string key)
        {
            return Values.TryGetValue(key, out var value) ? value : null;
        }
    }
}
