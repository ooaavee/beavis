using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace BeavisCli
{
    public static class LineFormatter
    {
        private const string ParagraphPadding = "   ";

        public static string[] CreateLines<TObj, TMember1, TMember2>(
            IEnumerable<TObj> objects, 
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            bool indent, 
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 2, indent);
        }
       
        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 3, indent);
        }

        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3, TMember4>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3),
                        [4] = GetName(obj, expression4)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3),
                    [4] = GetValue(obj, expression4)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 4, indent);
        }

        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3, TMember4, TMember5>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3),
                        [4] = GetName(obj, expression4),
                        [5] = GetName(obj, expression5)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3),
                    [4] = GetValue(obj, expression4),
                    [5] = GetValue(obj, expression5)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 5, indent);
        }

        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3),
                        [4] = GetName(obj, expression4),
                        [5] = GetName(obj, expression5),
                        [6] = GetName(obj, expression6)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3),
                    [4] = GetValue(obj, expression4),
                    [5] = GetValue(obj, expression5),
                    [6] = GetValue(obj, expression6)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 6, indent);
        }

        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3),
                        [4] = GetName(obj, expression4),
                        [5] = GetName(obj, expression5),
                        [6] = GetName(obj, expression6),
                        [7] = GetName(obj, expression7)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3),
                    [4] = GetValue(obj, expression4),
                    [5] = GetValue(obj, expression5),
                    [6] = GetValue(obj, expression6),
                    [7] = GetValue(obj, expression7)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 7, indent);
        }

        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TMember8>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            Expression<Func<TObj, TMember8>> expression8,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3),
                        [4] = GetName(obj, expression4),
                        [5] = GetName(obj, expression5),
                        [6] = GetName(obj, expression6),
                        [7] = GetName(obj, expression7),
                        [8] = GetName(obj, expression8)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3),
                    [4] = GetValue(obj, expression4),
                    [5] = GetValue(obj, expression5),
                    [6] = GetValue(obj, expression6),
                    [7] = GetValue(obj, expression7),
                    [8] = GetValue(obj, expression8)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 8, indent);
        }

        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TMember8, TMember9>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            Expression<Func<TObj, TMember8>> expression8,
            Expression<Func<TObj, TMember9>> expression9,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3),
                        [4] = GetName(obj, expression4),
                        [5] = GetName(obj, expression5),
                        [6] = GetName(obj, expression6),
                        [7] = GetName(obj, expression7),
                        [8] = GetName(obj, expression8),
                        [9] = GetName(obj, expression9)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3),
                    [4] = GetValue(obj, expression4),
                    [5] = GetValue(obj, expression5),
                    [6] = GetValue(obj, expression6),
                    [7] = GetValue(obj, expression7),
                    [8] = GetValue(obj, expression8),
                    [9] = GetValue(obj, expression9)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 9, indent);
        }

        public static string[] CreateLines<TObj, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TMember8, TMember9, TMember10>(
            IEnumerable<TObj> objects,
            Expression<Func<TObj, TMember1>> expression1,
            Expression<Func<TObj, TMember2>> expression2,
            Expression<Func<TObj, TMember3>> expression3,
            Expression<Func<TObj, TMember4>> expression4,
            Expression<Func<TObj, TMember5>> expression5,
            Expression<Func<TObj, TMember6>> expression6,
            Expression<Func<TObj, TMember7>> expression7,
            Expression<Func<TObj, TMember8>> expression8,
            Expression<Func<TObj, TMember9>> expression9,
            Expression<Func<TObj, TMember10>> expression10,
            bool indent,
            bool createHeader)
        {
            var rows = new List<Dictionary<int, string>>();

            foreach (TObj obj in objects)
            {
                if (createHeader && !rows.Any())
                {
                    var header = new Dictionary<int, string>
                    {
                        [1] = GetName(obj, expression1),
                        [2] = GetName(obj, expression2),
                        [3] = GetName(obj, expression3),
                        [4] = GetName(obj, expression4),
                        [5] = GetName(obj, expression5),
                        [6] = GetName(obj, expression6),
                        [7] = GetName(obj, expression7),
                        [8] = GetName(obj, expression8),
                        [9] = GetName(obj, expression9),
                        [10] = GetName(obj, expression10)
                    };
                    rows.Add(header);
                    rows.Add(CreateHeaderSeparator(header));
                }

                var row = new Dictionary<int, string>
                {
                    [1] = GetValue(obj, expression1),
                    [2] = GetValue(obj, expression2),
                    [3] = GetValue(obj, expression3),
                    [4] = GetValue(obj, expression4),
                    [5] = GetValue(obj, expression5),
                    [6] = GetValue(obj, expression6),
                    [7] = GetValue(obj, expression7),
                    [8] = GetValue(obj, expression8),
                    [9] = GetValue(obj, expression9),
                    [10] = GetValue(obj, expression10)
                };
                rows.Add(row);
            }

            return MakeLines(rows, 10, indent);
        }

        private static Dictionary<int, string> CreateHeaderSeparator(Dictionary<int, string> header)
        {
            var separator = new Dictionary<int, string>();

            for (int i = 1; i <= 10; i++)
            {
                if (!header.TryGetValue(i, out string title))
                {
                    break;
                }

                int len = title.Length;
                string line = len > 0 ? new string('-', len) : string.Empty;

                separator[i] = line;
            }

            return separator;
        }

        private static string GetValue<TObj, TMember>(TObj obj, Expression<Func<TObj, TMember>> expression)
        {
            Func<TObj, TMember> compiledDelegate = expression.Compile();
            TMember value = compiledDelegate(obj);

            if (value == null)
            {
                return string.Empty;
            }

            string text = value.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }

            return string.Empty;
        }

        private static string GetName<TObj, TMember>(TObj obj, Expression<Func<TObj, TMember>> expression)
        {
            MemberExpression memberExpr = (MemberExpression)expression.Body;
            string memberName = memberExpr.Member.Name;

            if (string.IsNullOrEmpty(memberName))
            {
                return string.Empty;
            }

            // insert spaces between words on a camel-cased token
            string name = Regex.Replace(memberName, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

            name = name.ToUpperInvariant();

            return name;
        }

        private static string[] MakeLines(List<Dictionary<int, string>> rows, int colCount, bool indent)
        {
            var lines = new List<string>();
            var maxLens = new Dictionary<int, int>();

            foreach (var row in rows)
            {
                for (var i = 1; i <= colCount; i++)
                {
                    maxLens.TryGetValue(i, out int maxLen);
                    var value = row[i];
                    var len = value.Length;
                    if (len >= maxLen)
                    {
                        maxLens[i] = len;
                    }
                }
            }

            foreach (var row in rows)
            {
                var buf = new StringBuilder();

                // left padding
                buf.Append(indent ? ParagraphPadding : "");

                for (var i = 1; i <= colCount; i++)
                {
                    var value = row[i];
                    var maxLen = maxLens[i];
                    var text = value + new string(' ', maxLen - value.Length + 2);
                    if (i > 1)
                    {
                        buf.Append(ParagraphPadding);
                    }
                    buf.Append(text);
                }

                lines.Add(buf.ToString());
            }

            return lines.ToArray();
        }      
    }
}
