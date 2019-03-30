namespace BeavisCli.JsInterop.Statements
{
    public class SetHistory : IStatement
    {
        private readonly string _js;

        public SetHistory(bool enabled)
        {
            _js = enabled ? 
                "terminal.history().enable();" : 
                "terminal.history().disable();";
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
