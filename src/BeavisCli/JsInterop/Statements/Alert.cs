using System.Text.Encodings.Web;

namespace BeavisCli.JsInterop.Statements
{
    /// <summary>
    /// This statement displays an alert box with a specified message and an OK button.
    /// </summary>
    public sealed class Alert : IStatement
    {
        private readonly string _message;

        public Alert(string message)
        {
            _message = JavaScriptEncoder.Default.Encode(message);
        }

        public string GetJs()
        {
            return $"alert('{_message}');";
        }
    }
}
