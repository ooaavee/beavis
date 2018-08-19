using System.Collections.Generic;
using System.Text;

namespace BeavisCli.JavaScriptStatements
{
    public class JavaScriptStatementComposition : IJavaScriptStatement
    {
        private readonly string _js;

        public JavaScriptStatementComposition(IEnumerable<IJavaScriptStatement> statements)
        {
            var js = new StringBuilder();

            foreach (IJavaScriptStatement statement in statements)
            {
                var code = statement.GetCode();

                js.Append(code);
                js.Append(";");
            }

            _js = js.ToString();
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
