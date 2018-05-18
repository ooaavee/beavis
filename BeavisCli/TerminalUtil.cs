using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public static class TerminalUtil
    {
        public static IEnumerable<string> MakeBeautifulLines(List<Tuple<string, string>> lines)
        {
            int maxLen = 0;

            foreach (Tuple<string, string> line in lines)
            {
                int len = line.Item1.Length;
                if (len > maxLen)
                {
                    maxLen = len;
                }
            }

            var texts = new List<string>();

            foreach (Tuple<string, string> line in lines)
            {
                var text = $"   {(line.Item1 + new string(' ', maxLen)).Substring(0, maxLen + 1)}   {line.Item2}";
                texts.Add(text);
            }

            return texts;
        }

    }
}
