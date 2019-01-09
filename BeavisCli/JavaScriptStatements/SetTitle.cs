using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public class SetTitle : IJavaScriptStatement
    {
        private readonly string _js;

        public SetTitle(string title)
        {
            _js = $"document.title = '{JavaScriptEncoder.Default.Encode(title)}'";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
