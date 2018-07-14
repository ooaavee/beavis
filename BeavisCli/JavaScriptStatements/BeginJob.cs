using System;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public sealed class BeginJob : IJavaScriptStatement
    {
        private readonly string _key;

        public BeginJob(string key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public string GetJavaScript()
        {
            return $"$ctrl.beginJob('{JavaScriptEncoder.Default.Encode(_key)}', terminal);";
        }
    }
}
