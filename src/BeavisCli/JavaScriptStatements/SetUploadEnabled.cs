namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement enables or disables the upload command.
    /// </summary>
    public sealed class SetUploadEnabled : IJavaScriptStatement
    {
        private readonly string _js;

        public SetUploadEnabled(bool enabled)
        {
            _js = $"window[\"__upload_enabled\"] = {enabled.ToString().ToLowerInvariant()};";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
