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
                js.Append(statement.GetCode());
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
