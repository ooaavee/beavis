namespace BeavisCli.JavaScriptStatements
{
    public class SetMask : IJavaScriptStatement
    {
        private readonly string _js;

        public SetMask(bool mask)
        {
            _js = $"terminal.set_mask({mask.ToString().ToLowerInvariant()});";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
