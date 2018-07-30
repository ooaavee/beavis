namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement reloads the terminal
    /// </summary>
    public sealed class Reload : IJavaScriptStatement
    {
        private readonly bool _forceGet;

        public Reload() : this(false)
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
