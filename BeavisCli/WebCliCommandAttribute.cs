using System;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebCliCommandAttribute : Attribute
    {
        public WebCliCommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Command name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; }
    }
}
