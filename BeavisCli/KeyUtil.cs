using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli
{
    public static class KeyUtil
    {
        public static string GenerateKey()
        {
            string value = Guid.NewGuid().ToString().Substring(0, 8);
            return value;
        }

        public static string GenerateKey(Func<string, bool> exists)
        {
            const int maxTries = 3;

            for (int i = 0; i < maxTries; i++)
            {
                string value = GenerateKey();
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