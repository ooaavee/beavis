using System;

namespace BeavisCli.Internal
{
    internal class WebCliApplicationMeta
    {
        public WebCliApplicationMeta(string name, string description)
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

        public static WebCliApplicationMeta Get(Type type)
        {
            WebCliApplicationMeta value = null;
            if (type.GetCustomAttributes(typeof(WebCliApplicationDefinitionAttribute), true) is WebCliApplicationDefinitionAttribute[] attributes && attributes.Length > 0)
            {
                WebCliApplicationDefinitionAttribute attribute = attributes[0];
                value = new WebCliApplicationMeta(attribute.Name, attribute.Description);
            }
            return value;
        }
    }
}
