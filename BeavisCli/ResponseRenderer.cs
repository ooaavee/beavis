using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli
{
    public static class ResponseRenderer
    {
        private const string ParagraphPadding = "   ";

        public static IEnumerable<string> FormatLines(IEnumerable<Tuple<string, string>> lines, bool indent = false)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            Tuple<string, string>[] items = lines.ToArray();

            int maxLen = 0;

            foreach (Tuple<string, string> item in items)
            {
                int len = item.Item1.Length;
                if (len > maxLen)
                {
                    maxLen = len;
                }
            }

            string leftPadding = indent ? ParagraphPadding : "";

            foreach (Tuple<string, string> item in items)
            {
                yield return $"{leftPadding}{(item.Item1 + new string(' ', maxLen)).Substring(0, maxLen)}{ParagraphPadding}{item.Item2}";
            }
        }

        public static IEnumerable<string> FormatLines(IEnumerable<string> lines, bool indent = false)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            string leftPadding = indent ? ParagraphPadding : "";

            foreach (string item in lines)
            {
                yield return $"{leftPadding}{item}";                
            }        
        }
    }
}
