namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement clears the terminal history.
    /// </summary>
    public sealed class ClearTerminalHistory : IJavaScriptStatement
    {
        public string GetJavaScript()
        {
            return "terminal.history().clear();";
        }
    }
}