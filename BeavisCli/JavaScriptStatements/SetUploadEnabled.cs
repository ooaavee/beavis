namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statement enables or disables the upload command.
    /// </summary>
    public sealed class SetUploadEnabled : IJavaScriptStatement
    {
        private readonly bool _enabled;

        public SetUploadEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        public string GetCode()
        {
           return $"window[\"__upload_enabled\"] = {_enabled.ToString().ToLowerInvariant()};";
        }
    }
}
