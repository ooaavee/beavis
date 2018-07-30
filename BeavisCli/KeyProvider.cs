using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli
{
    public static class KeyProvider
    {   
        public static string Create(Func<string, bool> valueChecker)
        {
            if (valueChecker == null)
            {
                throw new ArgumentNullException(nameof(valueChecker));
            }

            const int maxTries = 3;

            for (int i = 0; i < maxTries; i++)
            {
                string value = Guid.NewGuid().ToString().Substring(0, 8);

                bool exists = valueChecker(value);

                if (!exists)
                {
                    return value;
                }
            }

            throw new InvalidOperationException("Failed to generate a new random key.");
        }

        public static string Find(string candidate, Func<IEnumerable<string>> all)
        {
            if (string.IsNullOrEmpty(candidate))
            {
                return null;
            }
                     
            if (all == null)
            {
                throw new ArgumentNullException(nameof(all));
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