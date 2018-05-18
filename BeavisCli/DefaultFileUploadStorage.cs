using System.Threading.Tasks;

namespace BeavisCli
{
    public class DefaultFileUploadStorage : IFileUploadStorage
    {
        public Task UploadAsync(UploadedFile file, WebCliResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
