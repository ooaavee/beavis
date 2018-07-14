using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IFileUploadStorage
    {
        Task OnFileUploadedAsync(UploadedFile file, WebCliResponse response);
    }
}
