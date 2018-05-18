using System;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public class BeginJob : IJavaScriptStatement
    {
        private readonly string _key;

        public BeginJob(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _key = key;
        }

        public string GetJavaScript()
        {
            return $"$ctrl.beginJob('{JavaScriptEncoder.Default.Encode(_key)}', terminal);";
        }
    }
}
