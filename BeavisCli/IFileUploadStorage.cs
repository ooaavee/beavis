using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IFileUploadStorage
    {
        Task UploadAsync(UploadedFile file, WebCliResponse response);
    }
}
