namespace BeavisCli
{
    /// <summary>
    /// Error message
    /// </summary>
    public class ErrorMessage : ResponseMessage
    {
        public override string Type => "error";
    }
}
