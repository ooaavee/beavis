namespace BeavisCli.JavaScriptStatements
{
    public class SetHistory : IJavaScriptStatement
    {
        private readonly string _js;

        public SetHistory(bool enabled)
        {
            _js = enabled ? 
                "terminal.history().enable();" : 
                "terminal.history().disable();";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
