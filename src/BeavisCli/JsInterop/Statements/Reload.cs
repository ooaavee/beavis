namespace BeavisCli.JsInterop.Statements
{
    /// <summary>
    /// This statement reloads the terminal
    /// </summary>
    public sealed class Reload : IStatement
    {
        private readonly string _js;
       
        public Reload(bool forceGet = false)
        {
            _js = forceGet ? "location.reload(true);" : "location.reload();";
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
