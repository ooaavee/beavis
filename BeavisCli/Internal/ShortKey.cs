using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeavisCli.Internal
{
    internal static class ShortKey
    {
        public static string Create()
        {
            string value = Guid.NewGuid().ToString().Substring(0, 8);
            return value;
        }

        public static string Create(Func<string, bool> exists)
        {
            for (int i = 0; i < 3; i++)
            {
                string value = Create();
                if (!exists(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("Failed to generate a new random key.");
        }

        public static string FindMatching(string candidate, Func<IEnumerable<string>> existingValues)
        {
            string value = null;

            if (candidate.Length > 0)
            {
                string[] result = existingValues().Where(x => x.StartsWith(candidate)).ToArray();

                if (result.Length == 1)
                {
                    value = result[0];
                }
            }

            return value;
        }

    }
}
