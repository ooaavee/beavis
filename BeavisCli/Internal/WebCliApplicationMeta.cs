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

            if (type.GetCustomAttributes(typeof(WebCliApplicationAttribute), true) is WebCliApplicationAttribute[] items && items.Length > 0)
            {
                value = new WebCliApplicationMeta(items[0].Name, items[0].Description);
            }

            return value;
        }
    }
}
