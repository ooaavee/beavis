namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement reloads the terminal
    /// </summary>
    public sealed class Reload : IJavaScriptStatement
    {
        private readonly string _js;
       
        public Reload(bool forceGet = false)
        {
            _js = forceGet ? "location.reload(true);" : "location.reload();";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
