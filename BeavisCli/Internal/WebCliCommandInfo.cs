using System;

namespace BeavisCli.Internal
{
    internal class WebCliCommandInfo
    {
        private WebCliCommandInfo()
        {
        }

        /// <summary>
        /// Command name
        /// </summary>
        public string Name { get; private set; }

        public string FullName { get; private set; }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; private set; }

        public static WebCliCommandInfo Parse(Type type)
        {
            WebCliCommandInfo value = null;

            if (type.GetCustomAttributes(typeof(WebCliCommandAttribute), true) is WebCliCommandAttribute[] items && items.Length > 0)
            {
                value = new WebCliCommandInfo();
                value.Name = items[0].Name;
                value.FullName = items[0].FullName;


                // parsi täällä jostakin atribuutista description (vapaaehtoinen)

            }

            return value;
        }

    }
}
