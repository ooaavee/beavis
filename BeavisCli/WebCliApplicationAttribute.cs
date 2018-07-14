using System;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebCliApplicationAttribute : Attribute
    {
        public WebCliApplicationAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Application name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Application description
        /// </summary>
        public string Description { get; }
    }
}
