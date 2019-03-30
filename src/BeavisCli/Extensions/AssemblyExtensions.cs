using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeavisCli.Extensions
{
    public static class AssemblyExtensions
    {
        public static Dictionary<string, CommandBehaviour> GetCommands(this Assembly assembly)
        {
            var items = new Dictionary<string, CommandBehaviour>();

            Type[] types = assembly.GetExportedTypes();

            foreach (Type type in types)
            {
                if (!typeof(ICommand).IsAssignableFrom(type))
                {
                    continue;
                }

                if (!type.IsClass)
                {
                    continue;
                }

                if (type.IsAbstract)
                {
                    continue;
                }

                CommandInfo info = type.GetCommandInfo();

                items[info.Name] = new CommandBehaviour { ImplementationType = type };
            }

            return items;
        }
    }
}
