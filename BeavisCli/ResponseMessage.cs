using Newtonsoft.Json;

namespace BeavisCli
{
    /// <summary>
    /// Base class for all response messages
    /// </summary>
    public abstract class ResponseMessage
    {
        [JsonProperty("text")]
        public virtual string Text { get; set; }

        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    /// <summary>
    /// Error message
    /// </summary>
    public sealed class ErrorMessage : ResponseMessage
    {
        public ErrorMessage(string text)
        {
            Text = text ?? string.Empty;
        }

        public override string Type => "error";
    }

    /// <summary>
    /// Plain message
    /// </summary>
    public sealed class PlainMessage : ResponseMessage
    {
        public PlainMessage(string text)
        {
            Text = text ?? string.Empty;
        }

        public override string Type => "plain";
    }

    /// <summary>
    /// Success message
    /// </summary>
    public sealed class SuccessMessage : ResponseMessage
    {
        public SuccessMessage(string text)
        {
            Text = text ?? string.Empty;
        }

        public override string Type => "success";
    }
}
