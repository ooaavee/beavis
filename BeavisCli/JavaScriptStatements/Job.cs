using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement begins a job.
    /// </summary>
    public sealed class Job : IJavaScriptStatement
    {
        private readonly string _js;

        public Job(string key)
        {
            _js = $"$ctrl.job('{JavaScriptEncoder.Default.Encode(key)}', terminal);";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
