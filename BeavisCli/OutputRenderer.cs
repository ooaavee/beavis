using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli
{
    public static class OutputRenderer
    {
        private const string ParagraphPadding = "   ";

        public static IEnumerable<string> FormatLines(IEnumerable<Tuple<string, string>> lines, bool indent = false)
        {

            var items = lines.ToArray();

            int maxLen = 0;

            foreach (Tuple<string, string> item in items)
            {
                int len = item.Item1.Length;
                if (len > maxLen)
                {
                    maxLen = len;
                }
            }

            var leftPadding = indent ? ParagraphPadding : "";

            List<string> texts = new List<string>();

            foreach (Tuple<string, string> item in items)
            {
//                var s = $"{leftPadding}{(item.Item1 + new string(' ', maxLen)).Substring(0, maxLen + 1)}{ParagraphPadding}{item.Item2}";
                var s = $"{leftPadding}{(item.Item1 + new string(' ', maxLen)).Substring(0, maxLen )}{ParagraphPadding}{item.Item2}";

                texts.Add(s);

            }

            return texts;
        }

    }
}
