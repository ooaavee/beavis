using System;
using System.Collections.Concurrent;
using System.Linq;

namespace BeavisCli.Extensions
{
    public static class TypeExtensions
    {
        // we can safely use a static dictionary cache here, because these values doesn't change during runtime
        private static readonly ConcurrentDictionary<Type, CommandInfo> Cache = new ConcurrentDictionary<Type, CommandInfo>();

        public static CommandInfo GetCommandInfo(this Type type)
        {
            if (Cache.TryGetValue(type, out CommandInfo info))
            {
                return info;
            }

            if (type.GetCustomAttributes(typeof(CommandAttribute), true) is CommandAttribute[] attributes && attributes.Any())
            {
                CommandAttribute attribute = attributes.First();

                info = new CommandInfo
                {
                    Name = attribute.Name,
                    Description = attribute.Description,
                    LongDescription = attribute.LongDescription
                };

                Cache.TryAdd(type, info);
            }

            return info;
        }

    }
}
