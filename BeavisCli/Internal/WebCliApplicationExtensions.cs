using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace BeavisCli.Internal
{
    internal static class WebCliApplicationExtensions
    {
        // we can safely use a static dictionary cache here, because these values doesn't change during runtime
        private static readonly ConcurrentDictionary<Type, WebCliApplicationInfo> Cache = new ConcurrentDictionary<Type, WebCliApplicationInfo>();

        public static WebCliApplicationInfo GetInfo(this WebCliApplication application)
        {
            Type type = application.GetType();

            if (!Cache.TryGetValue(type, out WebCliApplicationInfo item))
            {
                item = WebCliApplicationInfo.Parse(type);
                if (item != null)
                {
                    Cache.TryAdd(type, item);
                }
            }

            return item;
        }

        private static readonly Assembly ThisAssembly = typeof(WebCliApplication).Assembly;

        public static bool IsBuiltIn(this WebCliApplication application)
        {
            Assembly applicationAssembly = application.GetType().Assembly;
            bool builtIn = applicationAssembly.Equals(ThisAssembly);
            return builtIn;
        }
    }

}