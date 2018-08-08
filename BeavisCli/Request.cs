using Newtonsoft.Json;

namespace BeavisCli
{
    public class Request
    {
        /// <summary>
        /// An input string from the web client.
        /// </summary>
        [JsonProperty("input")]
        public virtual string Input { get; set; }
    }
}
