using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class FileUploadStorage : IFileUploadStorage
    {
        public Task UploadAsync(UploadedFile file, WebCliResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
