using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IFileStorage
    {
        Task<string> StoreAsync(UploadedFile file);
    }
}
