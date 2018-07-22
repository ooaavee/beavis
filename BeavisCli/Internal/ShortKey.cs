using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli.Internal
{
    internal static class KeyProvider
    {
        public static string Create()
        {
            string value = Guid.NewGuid().ToString().Substring(0, 8);
            return value;
        }

        public static string Create(Func<string, bool> exists)
        {
            const int maxTryCount = 3;

            for (int i = 0; i < maxTryCount; i++)
            {
                string value = Create();
                if (!exists(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("Failed to generate a new random key.");
        }

        public static string Find(string candidate, Func<IEnumerable<string>> existing)
        {
            if (string.IsNullOrEmpty(candidate))
            {
                return null;
            }

            string[] matches = existing().Where(x => x.StartsWith(candidate)).ToArray();

            if (matches.Length == 1)
            {
                string value = matches[0];
                return value;
            }

            return null;
        }
    }
}
