using System.Collections.Generic;

namespace Beavis.Isolation.Contracts
{
    public class HttpRequestEnvelope
    {
        /// <summary>Gets or set the HTTP method.</summary>
        /// <returns>The HTTP method.</returns>
        public string Method { get; set; }

        /// <summary>Gets or set the HTTP request scheme.</summary>
        /// <returns>The HTTP request scheme.</returns>
        public string Scheme { get; set; }

        /// <summary>Returns true if the RequestScheme is https.</summary>
        /// <returns>true if this request is using https; otherwise, false.</returns>
        public bool IsHttps { get; set; }

        /// <summary>Gets or set the Host header. May include the port.</summary>
        /// <return>The Host header.</return>
        public string Host { get; set; }

        /// <summary>Gets or set the RequestPathBase.</summary>
        /// <returns>The RequestPathBase.</returns>
        public string PathBase { get; set; }

        /// <summary>Gets or set the request path from RequestPath.</summary>
        /// <returns>The request path from RequestPath.</returns>
        public string Path { get; set; }

        /// <summary>
        /// Gets or set the raw query string used to create the query collection in Request.Query.
        /// </summary>
        /// <returns>The raw query string.</returns>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets the query value collection parsed from Request.QueryString.
        /// </summary>
        /// <returns>The query value collection parsed from Request.QueryString.</returns>
        public Dictionary<string, string[]> Query { get; set; } = new Dictionary<string, string[]>();

        /// <summary>Gets or set the RequestProtocol.</summary>
        /// <returns>The RequestProtocol.</returns>
        public string Protocol { get; set; }

        /// <summary>Gets the request headers.</summary>
        /// <returns>The request headers.</returns>
        public Dictionary<string, string[]> Headers { get; set; } = new Dictionary<string, string[]>();

        /// <summary>Gets the collection of Cookies for this request.</summary>
        /// <returns>The collection of Cookies for this request.</returns>
        public List<string[]> Cookies { get; set; } = new List<string[]>();

        /// <summary>Gets or sets the Content-Length header</summary>
        public long? ContentLength { get; set; }

        /// <summary>Gets or sets the Content-Type header.</summary>
        /// <returns>The Content-Type header.</returns>
        public string ContentType { get; set; }

        /// <summary>Gets or set the RequestBody Stream.</summary>
        /// <returns>The RequestBody Stream.</returns>
        public byte[] Body { get; set; }

        /// <summary>Checks the content-type header for form types.</summary>
        public bool HasFormContentType { get; set; }

        /// <summary>Gets or sets the request body as a form.</summary>
        public Dictionary<string, string[]> Form { get; set; } = new Dictionary<string, string[]>();
    }
}