namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement clears the terminal history.
    /// </summary>
    public sealed class ClearTerminalHistory : IJavaScriptStatement
    {
        public string GetCode()
        {
            return "terminal.history().clear();";
        }
    }
}