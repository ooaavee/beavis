using System;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public class Alert : IJavaScriptStatement
    {
        private readonly string _message;

        public Alert(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _message = JavaScriptEncoder.Default.Encode(message); ;
        }

        public string GetJavaScript()
        {
            var js = $"alert('{_message}');";
            return js;
        }
    }
}
