namespace BeavisCli.JsInterop.Statements
{
    /// <summary>
    /// This statement clears the terminal.
    /// </summary>
    public sealed class ClearTerminal : IStatement
    {
        public string GetJs()
        {
            return "terminal.clear();";
        }
    }
}
