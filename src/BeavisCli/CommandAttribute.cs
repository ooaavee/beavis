using System;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name, string description, string longDescription = null)
        {
            Name = name;
            Description = description;
            LongDescription = longDescription;
        }

        /// <summary>
        /// Command name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description text
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Long description text
        /// </summary>
        public string LongDescription { get; }
    }
}
