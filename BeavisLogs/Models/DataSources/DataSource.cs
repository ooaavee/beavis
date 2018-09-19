using System.Collections.Generic;

namespace BeavisLogs.Models.DataSources
{
    public sealed class DataSource 
    {
        public DataSourceInfo Info { get; set; }

        /// <summary>
        /// <see cref="BeavisLogs.Drivers.IDriver"/> implementation type name
        /// </summary>
        public string DriverType { get; set; }

        /// <summary>
        /// Driver properties
        /// </summary>
        public DriverProperties DriverProperties { get; set; } = new DriverProperties();

        /// <summary>
        /// Allowed users
        /// </summary>
        public List<DataSourceUserAccess> Users { get; set; } = new List<DataSourceUserAccess>();
    }
}
