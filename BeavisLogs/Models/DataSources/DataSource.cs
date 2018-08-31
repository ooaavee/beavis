using System.Collections.Generic;
using BeavisLogs.Drivers;

namespace BeavisLogs.Models.DataSources
{
    public class DataSource 
    {
        /// <summary>
        /// Data source name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <see cref="IDriver"/> implementation type name
        /// </summary>
        public string DriverType { get; set; }

        /// <summary>
        /// Data source properties
        /// </summary>
        public DataSourceProperties Properties { get; set; }

        /// <summary>
        /// Allowed users
        /// </summary>
        public List<DataSourceUserAccess> Users { get; set; } = new List<DataSourceUserAccess>();
    }
}
