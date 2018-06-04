using Microsoft.AspNetCore.Http;

namespace Beavis.Isolation.Contracts
{
    public class ModuleRequest
    {
        public HttpRequestEnvelope HttpRequest { get; set; }

        /// <summary>
        /// TÄMÄ LÄHTEE POIS!!!
        /// </summary>
        public string Data { get; set; }

        public ModuleRequest()
        {
        }

        public ModuleRequest(HttpContext context)
        {
            HttpRequest = HttpContextUtil.ParseHttpRequest(context);
        }

        public static ModuleRequest Empty()
        {
            return new ModuleRequest();
        }
    }
}
