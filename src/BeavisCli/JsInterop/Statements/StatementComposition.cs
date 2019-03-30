using System.Collections.Generic;
using System.Text;

namespace BeavisCli.JsInterop.Statements
{
    public class StatementComposition : IStatement
    {
        private readonly string _js;

        public StatementComposition(IEnumerable<IStatement> statements)
        {
            var s = new StringBuilder();

            foreach (IStatement statement in statements)
            {
                s.Append(statement.GetJs());
                s.Append(";");
            }

            _js = s.ToString();
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
