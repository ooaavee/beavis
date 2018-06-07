using System.Collections.Generic;

namespace Beavis.Isolation.Contracts
{
    public class HttpResponseEnvelope
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string[]> Headers { get; set; } = new Dictionary<string, string[]>();
        public byte[] Body { get; set; }
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
        public List<string[]> Cookies { get; set; } = new List<string[]>();
        public bool HasStarted { get; set; }

        public bool HasRedirect { get; set; }
        public string RedirectLocation { get; set; }
        public bool IsRedirectPermanent { get; set; }
    }
}