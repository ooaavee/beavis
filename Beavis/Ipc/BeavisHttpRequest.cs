using Microsoft.AspNetCore.Http.Internal;

namespace Beavis.Ipc
{
    public sealed class BeavisHttpRequest : DefaultHttpRequest
    {
        public BeavisHttpRequest(BeavisHttpContext context, HttpRequestModel model) : base(context)
        {
        }
    }
}