using System;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebCliApplicationDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Application name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Application description
        /// </summary>
        public string Description { get; set; }
    }
}
