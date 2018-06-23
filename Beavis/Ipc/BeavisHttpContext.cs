using System;
using Microsoft.AspNetCore.Http;

namespace Beavis.Ipc
{
    public sealed class BeavisHttpContext : DefaultHttpContext, IDisposable
    {
        public BeavisHttpContext(HttpRequestModel request)
        {
            BeavisRequest = new BeavisHttpRequest(this, request);
            BeavisResponse = new BeavisHttpResponse(this);
        }

        internal BeavisHttpRequest BeavisRequest { get; set; }

        internal BeavisHttpResponse BeavisResponse { get; set; }

        public override HttpRequest Request => BeavisRequest;

        public override HttpResponse Response => BeavisResponse;

        public void Dispose()
        {           
            BeavisRequest?.Dispose();
            BeavisResponse?.Dispose();
        }
    }
}