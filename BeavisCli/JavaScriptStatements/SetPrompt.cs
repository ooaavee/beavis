using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public class SetPrompt : IJavaScriptStatement
    {
        private readonly string _js;

        public SetPrompt(string prompt)
        {
            _js = $"terminal.set_prompt('{JavaScriptEncoder.Default.Encode(prompt)}');";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
