using Microsoft.AspNetCore.Http;

namespace Beavis.Ipc
{
    public sealed class BeavisHttpContext : DefaultHttpContext
    {
        internal BeavisHttpRequest BeavisRequest { get; set; }

        internal BeavisHttpResponse BeavisResponse { get; set; }

        public override HttpRequest Request => BeavisRequest;

        public override HttpResponse Response => BeavisResponse;
    }
}