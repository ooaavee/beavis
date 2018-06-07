using Microsoft.AspNetCore.Http.Internal;

namespace Beavis.Http
{
    public sealed class ModuleHttpRequest : DefaultHttpRequest
    {
        public ModuleHttpRequest(ModuleHttpContext context) : base(context)
        {

        }
    }
}