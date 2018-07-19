using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IFileStorage
    {
        /// <summary>
        /// Stores a file into the file storage.
        /// </summary>
        /// <param name="file">a file to store</param>
        /// <returns>file id</returns>
        Task<string> StoreAsync(UploadedFile file);

        /// <summary>
        /// Removes a file from the file storage by using the file id.
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>removed file or null if we didn't remove anything</returns>
        Task<UploadedFile> RemoveAsync(string id);

        /// <summary>
        /// Gets all files and keys in the file storage.
        /// </summary>
        /// <returns>files and keys</returns>
        Task<IEnumerable<Tuple<string, UploadedFile>>> GetAllAsync();
    }
}
