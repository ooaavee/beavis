using System.Collections.Generic;
using System.Text;

namespace BeavisCli.JavaScriptStatements
{
    public class JavaScriptStatementComposition : IJavaScriptStatement
    {
        private readonly string _js;

        public JavaScriptStatementComposition(IEnumerable<IJavaScriptStatement> statements)
        {
            var s = new StringBuilder();

            foreach (IJavaScriptStatement statement in statements)
            {
                s.Append(statement.GetCode());
                s.Append(";");
            }

            _js = s.ToString();
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
