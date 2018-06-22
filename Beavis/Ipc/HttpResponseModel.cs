using System.Collections.Generic;

namespace Beavis.Ipc
{
    public class HttpResponseModel
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string[]> Headers { get; set; } = new Dictionary<string, string[]>();
        public byte[] Body { get; set; }
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
        public BeavisResponseCookies Cookies { get; set; }
        public bool HasRedirect { get; set; }
        public string RedirectLocation { get; set; }
        public bool? IsRedirectPermanent { get; set; }
    }
}