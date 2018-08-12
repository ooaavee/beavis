using System;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandDescriptionAttribute : Attribute
    {
        public CommandDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; }
    }
}
