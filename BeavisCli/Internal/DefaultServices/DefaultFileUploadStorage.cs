using System.Threading.Tasks;

namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultFileUploadStorage : IFileUploadStorage
    {
        public Task OnFileUploadedAsync(UploadedFile file, WebCliResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
