using System.Collections.Generic;
using BeavisLogs.Drivers;

namespace BeavisLogs.Models.DataSources
{
    public class DataSource 
    {
        /// <summary>
        /// Data source id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Data source name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <see cref="IDriver"/> implementation type name
        /// </summary>
        public string DriverType { get; set; }

        /// <summary>
        /// Driver properties
        /// </summary>
        public DriverProperties Properties { get; set; }

        /// <summary>
        /// Allowed users
        /// </summary>
        public List<DataSourceUserAccess> Users { get; set; } = new List<DataSourceUserAccess>();
    }
}
