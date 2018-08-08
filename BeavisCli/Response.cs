using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public class Response
    {
        public Response(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        /// <summary>
        /// Occurs just before sending the response.
        /// </summary>
        public event EventHandler Sending;

        /// <summary>
        /// HTTP context
        /// </summary>
        [JsonIgnore]
        public virtual HttpContext HttpContext { get; }

        /// <summary>
        /// Response messages
        /// </summary>
        [JsonProperty("messages")]
        public virtual List<ResponseMessage> Messages { get; } = new List<ResponseMessage>();

        /// <summary>
        /// JavaScript statements that will be evaluated on the client-side.
        /// </summary>
        [JsonProperty("statements")]
        public List<string> Statements { get; } = new List<string>();

        /// <summary>
        /// Raises the <see cref="Sending"/> event.
        /// </summary>
        public void OnSending()
        {
            var handler = Sending;
            if (handler != null)
            {
                lock (handler)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}
