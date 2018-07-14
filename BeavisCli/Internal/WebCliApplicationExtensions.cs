using System;
using System.Collections.Concurrent;

namespace BeavisCli.Internal
{
    internal static class WebCliApplicationExtensions
    {
        private static readonly ConcurrentDictionary<Type, WebCliApplicationMeta> Cache = new ConcurrentDictionary<Type, WebCliApplicationMeta>();

        public static WebCliApplicationMeta Meta(this WebCliApplication application)
        {
            Type type = application.GetType();
            if (!Cache.TryGetValue(type, out WebCliApplicationMeta value))
            {
                value = WebCliApplicationMeta.Get(type);
                if (value != null)
                {
                    Cache.TryAdd(type, value);
                }
            }
            return value;
        }
    }
}