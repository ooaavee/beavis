using System.Text.Encodings.Web;

namespace BeavisCli.JsInterop.Statements
{
    public class SetTitle : IStatement
    {
        private readonly string _js;

        public SetTitle(string title)
        {
            _js = $"document.title = '{JavaScriptEncoder.Default.Encode(title)}'";
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
