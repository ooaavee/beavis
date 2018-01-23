using System;
using System.Text;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public class SetTerminalCompletionDictionary : IJavaScriptStatement
    {
        private readonly string[] _names;

        public SetTerminalCompletionDictionary(string[] names)
        {
            if (names == null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            _names = names;
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
