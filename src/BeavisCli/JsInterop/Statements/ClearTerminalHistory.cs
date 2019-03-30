namespace BeavisCli.JsInterop.Statements
{
    /// <summary>
    /// This statement clears the terminal history.
    /// </summary>
    public sealed class ClearTerminalHistory : IStatement
    {
        public string GetJs()
        {
            return "terminal.history().clear();";
        }
    }
}