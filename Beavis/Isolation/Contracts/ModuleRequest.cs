using Beavis.Http;
using Microsoft.AspNetCore.Http;

namespace Beavis.Isolation.Contracts
{
    public class ModuleRequest
    {
        public HttpRequestEnvelope Content { get; set; }

        /// <summary>
        /// TÄMÄ LÄHTEE POIS!!!
        /// </summary>
        public string Data { get; set; }

        public ModuleRequest()
        {
        }

        public ModuleRequest(HttpContext context)
        {
            Content = ModuleHttpContext.ParseRequest(context);
        }

        public static ModuleRequest Empty()
        {
            return new ModuleRequest();
        }
    }
}
