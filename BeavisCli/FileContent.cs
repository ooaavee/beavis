using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BeavisCli
{
    /// <summary>
    /// Represents a file uploaded from the client-side.
    /// </summary>
    public class FileContent
    {
        /// <summary>
        /// File name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// File type (MIME type)
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// File data url (https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/Data_URIs)
        /// </summary>
        [JsonProperty("dataUrl")]
        public string DataUrl { get; set; }

        /// <summary>
        /// Get file bytes.
        /// </summary>
        /// <returns>files bytes</returns>
        public byte[] GetBytes()
        {
            if (string.IsNullOrEmpty(DataUrl))
            {
                throw new InvalidOperationException($"{nameof(DataUrl)} is null or empty string.");
            }

            string base64Data = Regex.Match(DataUrl, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            byte[] binData = Convert.FromBase64String(base64Data);

            using (var stream = new MemoryStream(binData))
            {
                byte[] bytes = stream.ToArray();
                return bytes;
            }
        }
    }
}
