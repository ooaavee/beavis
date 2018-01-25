namespace BeavisCli
{
    /// <summary>
    /// Information message
    /// </summary>
    public sealed class InformationMessage : ResponseMessage
    {
        public InformationMessage(string text)
        {
            Text = text;
        }
        public override string Type => "information";
    }
}