using Newtonsoft.Json;

namespace BeavisCli
{
    /// <summary>
    /// This is the base class for all response messages.
    /// </summary>
    public abstract class ResponseMessage
    {
        [JsonProperty("text")]
        public virtual string Text { get; set; }

        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }
}
