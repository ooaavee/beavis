using System.Text.Encodings.Web;

namespace BeavisCli.JsInterop.Statements
{
    /// <summary>
    /// This statement begins a job.
    /// </summary>
    public sealed class BeginJob : IStatement
    {
        private readonly string _js;

        public BeginJob(string key)
        {
            _js = $"$ctrl.beginJob('{JavaScriptEncoder.Default.Encode(key)}', null);";
        }

        public string GetJs()
        {
            return _js;
        }       
    }
}
