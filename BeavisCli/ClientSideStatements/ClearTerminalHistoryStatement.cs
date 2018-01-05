namespace BeavisCli.ClientSideStatements
{
    /// <summary>
    /// This statement clears the terminal history.
    /// </summary>
    public class ClearTerminalHistoryStatement : IClientSideStatement
    {
        public string GetJavaScript()
        {
            return "terminal.history().clear();";
        }
    }
}