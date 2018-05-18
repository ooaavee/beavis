using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal
{
    internal class NullVoidService : IUnauthorizedHandler, ITerminalInitializer, IFileUploadStorage
    {
        public void OnUnauthorized(WebCliContext context) { }
        public void Initialize(HttpContext context, WebCliResponse response) { }

        public Task UploadAsync(UploadedFile file, WebCliResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
