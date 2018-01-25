namespace BeavisCli
{
    /// <summary>
    /// Error message
    /// </summary>
    public sealed class ErrorMessage : ResponseMessage
    {
        public ErrorMessage(string text)
        {
            Text = text;
        }

        public override string Type => "error";
    }
}