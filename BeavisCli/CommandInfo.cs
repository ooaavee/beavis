using System;
using System.Collections.Concurrent;
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


        // we can safely use a static dictionary cache here, because these values doesn't change during runtime
        private static readonly ConcurrentDictionary<Type, CommandInfo> ResolvedInfo = new ConcurrentDictionary<Type, CommandInfo>();

        public static CommandInfo Get(ICommand cmd)
        {
            return Get(cmd.GetType());
        }

        public static CommandInfo Get(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!ResolvedInfo.TryGetValue(type, out CommandInfo info))
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

                    ResolvedInfo.TryAdd(type, info);
                }
            }

            return info;
        }
    }
}
