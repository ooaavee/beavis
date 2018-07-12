using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal
{
    /// <summary>
    /// An internal backup service that does nothing.
    /// </summary>
    internal class NullVoidService : IUnauthorizedHandler, ITerminalInitializer, IFileUploadStorage, ITerminalGreeter
    {
        void IUnauthorizedHandler.OnUnauthorized(WebCliContext context)
        {
        }

        void ITerminalInitializer.Initialize(HttpContext context, WebCliResponse response)
        {
        }

        Task IFileUploadStorage.UploadAsync(UploadedFile file, WebCliResponse response)
        {
            return Task.CompletedTask;
        }

        void ITerminalGreeter.SayGreetings(HttpContext context, WebCliResponse response)
        {
        }
    }
}
