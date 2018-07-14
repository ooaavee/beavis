using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IUploadStorage
    {
        Task OnFileUploadedAsync(UploadedFile file, WebCliResponse response);
    }
}
