using System;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement begins a job.
    /// </summary>
    public sealed class Job : IJavaScriptStatement
    {
        private readonly string _key;

        public Job(string key)
        {
            _key = key;
        }

        public string GetCode()
        {
            return $"$ctrl.job('{JavaScriptEncoder.Default.Encode(_key)}', terminal);";
        }
    }
}
