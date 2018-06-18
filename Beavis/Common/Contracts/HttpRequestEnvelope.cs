using System.Collections.Generic;

namespace Beavis.Isolation.Contracts
{
    public class HttpRequestEnvelope
    {
        public string Method { get; set; }
        public string Scheme { get; set; }
        public bool IsHttps { get; set; }
        public string Host { get; set; }
        public string PathBase { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public Dictionary<string, string[]> Query { get; set; } = new Dictionary<string, string[]>();
        public string Protocol { get; set; }
        public Dictionary<string, string[]> Headers { get; set; } = new Dictionary<string, string[]>();
        public List<string[]> Cookies { get; set; } = new List<string[]>();
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
        public byte[] Body { get; set; }
        public bool HasFormContentType { get; set; }
        public Dictionary<string, string[]> Form { get; set; } = new Dictionary<string, string[]>();
    }
}