using System;

namespace BeavisCli.Internal
{
    internal class WebCliCommandInfo
    {
        public WebCliCommandInfo(string name, string description)
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

        public static WebCliCommandInfo Parse(Type type)
        {
            WebCliCommandInfo value = null;

            if (type.GetCustomAttributes(typeof(WebCliCommandAttribute), true) is WebCliCommandAttribute[] items && items.Length > 0)
            {
                value = new WebCliCommandInfo(items[0].Name, items[0].Description);
            }

            return value;
        }

        public static WebCliCommandInfo Parse<TWebCliCommand>() where TWebCliCommand : WebCliCommand
        {
            return Parse(typeof(TWebCliCommand));
        }
    }
}
