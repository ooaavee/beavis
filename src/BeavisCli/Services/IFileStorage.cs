using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public interface IFileStorage
    {
        /// <summary>
        /// Stores a file into the file storage.
        /// </summary>
        /// <param name="file">a file to store</param>
        /// <returns>file id</returns>
        Task<string> StoreAsync(FileContent file);

        /// <summary>
        /// Removes a file from the file storage by using the file id.
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>removed file or null if we didn't remove anything</returns>
        Task<FileContent> RemoveAsync(string id);

        /// <summary>
        /// Removes all files from the file storage.
        /// </summary>
        /// <returns>number of removed files</returns>
        Task<int> RemoveAllAsync();

        /// <summary>
        /// Gets all files in the file storage.
        /// </summary>
        /// <returns>all files</returns>
        Task<IEnumerable<Tuple<string, FileContent>>> GetAllAsync();

        /// <summary>
        /// Gets a file from the file storage by using the file .id
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>file or null if not found</returns>
        Task<FileContent> GetAsync(string id);
    }
}
