using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisLogs.Models.DataSources;

namespace BeavisLogs.Models.Users
{
    public class DataSourceGroup
    {
        public string Id { get; set; }

        /// <summary>
        /// Data source identifiers who are the members of this group.
        /// </summary>
        public List<string> Members { get; set; } = new List<string>();
    }
}
