using Newtonsoft.Json;

namespace BeavisCli
{
    /// <summary>
    /// Represents a file uploaded from the client-side.
    /// </summary>
    public class UploadedFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("dataUrl")]
        public string DataUrl { get; set; }
    }
}
