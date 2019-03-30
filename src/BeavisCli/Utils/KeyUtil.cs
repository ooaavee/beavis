using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli.Utils
{
    internal static class KeyUtil
    {
        public static string NewKey(Func<string, bool> exists)
        {
            const int maxTries = 3;

            for (int i = 0; i < maxTries; i++)
            {
                string value = Guid.NewGuid().ToString().Substring(0, 8);

                if (!exists(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("Failed to generate a new random key.");
        }

        public static string FindKey(string candidate, Func<IEnumerable<string>> all)
        {
            if (string.IsNullOrEmpty(candidate))
            {
                return null;
            }

            string[] matches = all().Where(x => x.StartsWith(candidate)).ToArray();

            if (matches.Length == 1)
            {
                return matches[0];
            }

            return null;
        }
    }
}