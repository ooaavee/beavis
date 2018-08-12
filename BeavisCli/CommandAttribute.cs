using System;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name, string description)
        {
            Name = name;
            FullName = description;
        }

        /// <summary>
        /// Command name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Command full name
        /// </summary>
        public string FullName { get; }
    }
}
