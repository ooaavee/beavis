using System.Text.Encodings.Web;

namespace BeavisCli.JsInterop.Statements
{
    public class SetPrompt : IStatement
    {
        private readonly string _js;

        public SetPrompt(string prompt)
        {
            _js = $"terminal.set_prompt('{JavaScriptEncoder.Default.Encode(prompt)}');";
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
