using System.Collections.Generic;

namespace BeavisLogs.Models.Users
{
    public class UserInfo
    {
        /// <summary>
        /// User's data source groups
        /// </summary>
        public List<DataSourceGroup> DataSourceGroups { get; set; } = new List<DataSourceGroup>();

        /// <summary>
        /// User's query templates
        /// </summary>
        public List<QueryTemplate> QueryTemplates { get; set; } = new List<QueryTemplate>();
    }
}
