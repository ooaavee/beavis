using System.Collections.Generic;

namespace BeavisCli
{
    public class JavaScriptStatementCollection : List<string>
    {
        public virtual void Add(IJavaScriptStatement statement)
        {
            string code = statement.GetCode();
            Add(code);
        }
    }
}