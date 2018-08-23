using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement begins a job.
    /// </summary>
    public sealed class BeginJob : IJavaScriptStatement
    {
        private readonly string _js;

        public BeginJob(string key)
        {
            _js = $"$ctrl.beginJob('{JavaScriptEncoder.Default.Encode(key)}', null);";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
