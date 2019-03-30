using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace BeavisCli.JsInterop.Statements
{
    /// <summary>
    /// This statement notifies which commands are known by the terminal tab completion.
    /// </summary>
    public sealed class SetTabCompletionCommands : IStatement
    {
        private readonly string _js;

        public SetTabCompletionCommands(IEnumerable<string> names)
        {
            var counter = 0;
            var s = new StringBuilder();

            s.Append("[");

            foreach (string name in names)
            {
                if (counter > 0)
                {
                    s.Append(", ");
                }
                s.Append($"'{JavaScriptEncoder.Default.Encode(name)}'");
                counter++;
            }

            s.Append("]");

            _js = $"window[\"__terminal_completion\"] = {s};";
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
