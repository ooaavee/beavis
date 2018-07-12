using System;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public sealed class Alert : IJavaScriptStatement
    {
        private readonly string _message;

        public Alert(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _message = JavaScriptEncoder.Default.Encode(message);
        }

        public string GetJavaScript()
        {
            return $"alert('{_message}');";
        }
    }
}
