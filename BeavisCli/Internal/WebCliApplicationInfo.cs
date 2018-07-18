using System;

namespace BeavisCli.Internal
{
    internal class WebCliApplicationInfo
    {
        public WebCliApplicationInfo(string name, string description)
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

        public static WebCliApplicationInfo Parse(Type type)
        {
            WebCliApplicationInfo value = null;

            if (type.GetCustomAttributes(typeof(WebCliApplicationAttribute), true) is WebCliApplicationAttribute[] items && items.Length > 0)
            {
                value = new WebCliApplicationInfo(items[0].Name, items[0].Description);
            }

            return value;
        }

        public static WebCliApplicationInfo Parse<TWebCliApplication>() where TWebCliApplication : WebCliApplication
        {
            return Parse(typeof(TWebCliApplication));
        }
    }
}
