using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace BeavisCli.Extensions
{
    public static class TypeExtensions
    {
        // we can safely use a static dictionary cache here, because these values does not change during runtime
        private static readonly ConcurrentDictionary<Type, CommandInfo> Cache = new ConcurrentDictionary<Type, CommandInfo>();

        public static CommandInfo GetCommandInfo(this Type type)
        {
            if (!Cache.TryGetValue(type, out CommandInfo info))
            {
                CommandAttribute cmd = type.GetCustomAttribute<CommandAttribute>();
                if (cmd != null)
                {
                    info = new CommandInfo
                    {
                        Name = cmd.Name,
                        Description = cmd.Description,
                        LongDescription = cmd.LongDescription
                    };
             
                    Cache.TryAdd(type, info);
                }
            }

            return info;
        }

    }
}
