using System.IO;
using Microsoft.AspNetCore.Http.Internal;

namespace Beavis.Http
{
    public sealed class ModuleHttpResponse : DefaultHttpResponse
    {
        public ModuleHttpResponse(ModuleHttpContext context) : base(context)
        {
            Body = new MemoryStream();
        }

    }
}