using System.Threading.Tasks;

namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultUploadStorage : IUploadStorage
    {
        public Task OnFileUploadedAsync(UploadedFile file, WebCliResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
