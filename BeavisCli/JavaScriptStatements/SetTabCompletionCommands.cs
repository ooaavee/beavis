using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement notifies which commands are known by the terminal tab completion.
    /// </summary>
    public sealed class SetTabCompletionCommands : IJavaScriptStatement
    {
        private readonly string[] _names;

        public SetTabCompletionCommands(IEnumerable<string> names)
        {
            _names = names.ToArray();
        }

        public string GetCode()
        {
            var s  = new StringBuilder();
            s.Append("[");
            for (var i = 0; i < _names.Length; i++)
            {
                if (i > 0)
                {
                    s.Append(", ");
                }
                s.Append($"'{JavaScriptEncoder.Default.Encode(_names[i])}'");
            }
            s.Append("]");
            var js = $"window[\"__terminal_completion\"] = {s};";
            return js;
        }
    }
}
