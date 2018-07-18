using System;
using System.Threading.Tasks;

namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultFileStorage : IFileStorage
    {
        public DefaultFileStorage()
        {

        }

        public Task<string> StoreAsync(UploadedFile file)
        {
            string id = Guid.NewGuid().ToString().Substring(0, 8);

            return Task.FromResult(id);
        }
    }
}
