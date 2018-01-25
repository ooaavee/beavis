namespace BeavisCli
{
    /// <summary>
    /// Information message
    /// </summary>
    public sealed class SuccessMessage : ResponseMessage
    {
        public SuccessMessage(string text)
        {
            Text = text;
        }

        public override string Type => "success";
    }
}