using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public sealed class SetTerminalCompletionDictionary : IJavaScriptStatement
    {
        private readonly string[] _names;

        public SetTerminalCompletionDictionary(IEnumerable<string> names)
        {
            if (names == null)
            {
                throw new ArgumentNullException(nameof(names));
            }
    
            _names = names.ToArray();
        }

        public string GetJavaScript()
        {
            var buf  = new StringBuilder();

            buf.Append("[");

            for (var i = 0; i < _names.Length; i++)
            {
                if (i > 0)
                {
                    buf.Append(", ");
                }
                buf.Append($"'{JavaScriptEncoder.Default.Encode(_names[i])}'");
            }

            buf.Append("]");

            var js = $"window[\"__terminal_completion\"] = {buf};";

            return js;
        }
    }
}
