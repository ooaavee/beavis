namespace BeavisCli.JsInterop.Statements
{
    public class SetMask : IStatement
    {
        private readonly string _js;

        public SetMask(bool mask)
        {
            _js = $"terminal.set_mask({mask.ToString().ToLowerInvariant()});";
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
