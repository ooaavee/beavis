using System;
using System.Collections.Concurrent;
using System.Linq;

namespace BeavisCli
{
    public sealed class CommandInfo
    {
        // we can safely use a static dictionary cache here, because these values doesn't change during runtime
        private static readonly ConcurrentDictionary<Type, CommandInfo> Known = new ConcurrentDictionary<Type, CommandInfo>();

        private CommandInfo() { }

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

        public static CommandInfo Get(ICommand cmd)
        {
            return Get(cmd.GetType());
        }

        public static CommandInfo Get(Type type)
        {
            if (!Known.TryGetValue(type, out CommandInfo info))
            {
                if (type.GetCustomAttributes(typeof(CommandAttribute), true) is CommandAttribute[] attrs1 && attrs1.Any())
                {
                    info = new CommandInfo
                    {
                        Name = attrs1[0].Name,
                        FullName = attrs1[0].FullName
                    };

                    if (type.GetCustomAttributes(typeof(CommandDescriptionAttribute), true) is CommandDescriptionAttribute[] attrs2 && attrs2.Any())
                    {
                        info.Description = attrs2[0].Description;
                    }

                    Known.TryAdd(type, info);
                }
            }

            return info;
        }
    }
}
