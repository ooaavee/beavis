using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
    public sealed class QueueJob : IJavaScriptStatement
    {
        private readonly string _js;

        public QueueJob(string key)
        {
            _js = $"$ctrl.queueJob('{JavaScriptEncoder.Default.Encode(key)}', null);";
        }

        public QueueJob(string key, IJavaScriptStatement js)
        {
            string code = JavaScriptEncoder.Default.Encode(js.GetCode());

            _js = $"$ctrl.queueJob('{JavaScriptEncoder.Default.Encode(key)}', '{code}');";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
