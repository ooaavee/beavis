using System.Text.Encodings.Web;

namespace BeavisCli.JsInterop.Statements
{
    public sealed class QueueJob : IStatement
    {
        private readonly string _js;

        public QueueJob(string key)
        {
            _js = $"$ctrl.queueJob('{JavaScriptEncoder.Default.Encode(key)}', null);";
        }

        public QueueJob(string key, IStatement statement)
        {
            string code = JavaScriptEncoder.Default.Encode(statement.GetJs());

            _js = $"$ctrl.queueJob('{JavaScriptEncoder.Default.Encode(key)}', '{code}');";
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
