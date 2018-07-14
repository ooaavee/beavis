using System;
using System.Collections.Concurrent;

namespace BeavisCli.Internal
{
    internal static class WebCliApplicationExtensions
    {
        // we can safely use a static dictionary cache here, because these values doesn't change during runtime
        private static readonly ConcurrentDictionary<Type, WebCliApplicationMeta> Cache = new ConcurrentDictionary<Type, WebCliApplicationMeta>();

        public static WebCliApplicationMeta Meta(this WebCliApplication application)
        {
            Type type = application.GetType();

            if (!Cache.TryGetValue(type, out WebCliApplicationMeta item))
            {
                item = WebCliApplicationMeta.Get(type);
                if (item != null)
                {
                    Cache.TryAdd(type, item);
                }
            }

            return item;
        }
    }
}