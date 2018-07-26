namespace BeavisCli.JavaScriptStatements
{
    public sealed class Reload : IJavaScriptStatement
    {
        private readonly bool _forceGet;

        public Reload()
        {
        }

        public Reload(bool forceGet)
        {
            _forceGet = forceGet;
        }

        public string GetJavaScript()
        {
            return _forceGet ?
                $"location.reload({_forceGet.ToString().ToLowerInvariant()});" :
                "location.reload();";
        }
    }
}
