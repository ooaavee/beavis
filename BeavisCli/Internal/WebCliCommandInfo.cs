using System;
using System.Linq;

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

        /// <summary>
        /// Command full name
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; private set; }

        public static WebCliCommandInfo FromType(Type type)
        {
            WebCliCommandInfo value = null;

            if (type.GetCustomAttributes(typeof(WebCliCommandAttribute), true) is WebCliCommandAttribute[] attrs1 && attrs1.Any())
            {
                value = new WebCliCommandInfo();
                value.Name = attrs1[0].Name;
                value.FullName = attrs1[0].FullName;

                if (type.GetCustomAttributes(typeof(WebCliCommandDescriptionAttribute), true) is WebCliCommandDescriptionAttribute[] attrs2 && attrs2.Any())
                {
                    value.Description = attrs2[0].Description;
                }
            }

            return value;
        }
    }
}
