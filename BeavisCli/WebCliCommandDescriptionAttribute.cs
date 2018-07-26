using System;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebCliCommandDescriptionAttribute : Attribute
    {
        public WebCliCommandDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; }
    }
}
