using Newtonsoft.Json;

namespace BeavisCli
{
    public class ApplicationExecutionRequest
    {
        /// <summary>
        /// An input string from the web client.
        /// </summary>
        [JsonProperty("input")]
        public string Input { get; set; }     
    }
}
