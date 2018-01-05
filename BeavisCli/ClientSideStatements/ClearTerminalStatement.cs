namespace BeavisCli.ClientSideStatements
{
    /// <summary>
    /// This statement clears the terminal.
    /// </summary>
    public class ClearTerminalStatement : IClientSideStatement
    {
        public string GetJavaScript()
        {
            return "terminal.clear();";
        }
    }
}
