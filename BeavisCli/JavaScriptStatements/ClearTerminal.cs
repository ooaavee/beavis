namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement clears the terminal.
    /// </summary>
    public sealed class ClearTerminal : IJavaScriptStatement
    {
        public string GetCode()
        {
            return "terminal.clear();";
        }
    }
}
