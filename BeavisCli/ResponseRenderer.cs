using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli
{
    public static class ResponseRenderer
    {
        private const string ParagraphPadding = "   ";

        public static IEnumerable<string> FormatLines(IEnumerable<Tuple<string, string, string>> lines, bool indent = false)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            Tuple<string, string, string>[] items = lines.ToArray();

            int maxLen1 = 0;
            int maxLen2 = 0;

            foreach (Tuple<string, string, string> item in items)
            {                
                int len1 = item.Item1.Length;
                if (len1 > maxLen1)
                {
                    maxLen1 = len1;
                }

                int len2 = item.Item2.Length;
                if (len2 > maxLen2)
                {
                    maxLen2 = len2;
                }
            }

            string leftPadding = indent ? ParagraphPadding : "";

            foreach (Tuple<string, string, string> item in items)
            {
                string line = $"{leftPadding}{Col(item.Item1, maxLen1)}{ParagraphPadding}{Col(item.Item2, maxLen2)}{ParagraphPadding}{item.Item3}";
                yield return line;
            }
        }

        public static IEnumerable<string> FormatLines(IEnumerable<Tuple<string, string>> lines, bool indent = false)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            Tuple<string, string>[] items = lines.ToArray();

            int maxLen1 = 0;

            foreach (Tuple<string, string> item in items)
            {
                int len1 = item.Item1.Length;
                if (len1 > maxLen1)
                {
                    maxLen1 = len1;
                }
            }

            string leftPadding = indent ? ParagraphPadding : "";

            foreach (Tuple<string, string> item in items)
            {
                string line = $"{leftPadding}{Col(item.Item1, maxLen1)}{ParagraphPadding}{item.Item2}";
                yield return line;
            }
        }

        ////public static IEnumerable<string> FormatLines(IEnumerable<string> lines, bool indent = false)
        ////{
        ////    if (lines == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(lines));
        ////    }

        ////    string leftPadding = indent ? ParagraphPadding : "";

        ////    foreach (string line in lines)
        ////    {
        ////        yield return $"{leftPadding}{line}";                
        ////    }        
        ////}

        private static string Col(string text, int len)
        {
            string col = text + new string(' ', len - text.Length + 2);
            return col;
        }
    }
}
