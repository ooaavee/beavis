using System;
using System.Linq;

namespace BeavisCli
{
    public sealed class CommandInfo
    {
        private CommandInfo()
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

        public static CommandInfo FromType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            CommandInfo value = null;

            if (type.GetCustomAttributes(typeof(CommandAttribute), true) is CommandAttribute[] attrs1 && attrs1.Any())
            {
                value = new CommandInfo();
                value.Name = attrs1[0].Name;
                value.FullName = attrs1[0].FullName;

                if (type.GetCustomAttributes(typeof(CommandDescriptionAttribute), true) is CommandDescriptionAttribute[] attrs2 && attrs2.Any())
                {
                    value.Description = attrs2[0].Description;
                }
            }

            return value;
        }
    }
}
