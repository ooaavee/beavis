namespace BeavisCli.JavaScriptStatements
{
    public class SetUploadEnabled : IJavaScriptStatement
    {
        private readonly bool _enabled;

        public SetUploadEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        public string GetJavaScript()
        {
           return $"window[\"__upload_enabled\"] = {_enabled.ToString().ToLowerInvariant()};";
        }
    }
}
