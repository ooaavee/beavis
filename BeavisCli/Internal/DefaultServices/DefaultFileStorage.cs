using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultFileStorage : IFileStorage
    {
        private readonly ILogger<DefaultFileStorage> _logger;

        private readonly ConcurrentDictionary<string, FileContent> _files = new ConcurrentDictionary<string, FileContent>();

        public DefaultFileStorage(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DefaultFileStorage>();
        }

        public Task<FileId> StoreAsync(FileContent file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            // create id for the file
            string id = KeyProvider.Create(s => _files.ContainsKey(s));

            // store file into memory
            bool succeed = _files.TryAdd(id, file);

            if (succeed)
            {
                _logger.LogDebug($"Stored a file by using the id {id}.");
            }
            else
            {
                var message = $"Failed to store a file by using the id {id}.";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            // return file id
            return Task.FromResult(new FileId(id));
        }

        public Task<FileContent> RemoveAsync(FileId id)
        {
            FileContent file = null;

            // try to find the real id
            string real = KeyProvider.Find(id.Value, () => _files.Keys);

            if (real != null)
            {
                _logger.LogDebug($"{id} -> {real} will be used when removing the file.");

                // try to remove the file by using the real id
                bool succeed = _files.TryRemove(real, out file);

                if (succeed)
                {
                    _logger.LogDebug($"Successfully removed a file by using the id {id} -> {real}.");
                }
                else
                {
                    _logger.LogError($"Failed to remove a file by using the id {id} -> {real}.");
                }
            }
            else
            {
                _logger.LogDebug($"Unable to find a file to remove by using the id {id}.");
            }

            // return the removed file or null if we didn't remove anything
            return Task.FromResult(file);
        }

        public Task<int> RemoveAllAsync()
        {
            lock (_files)
            {
                int count = _files.Count;
                _files.Clear();
                return Task.FromResult(count);
            }
        }

        public Task<IEnumerable<Tuple<FileId, FileContent>>> GetAllAsync()
        {
            var result = new List<Tuple<FileId, FileContent>>();

            foreach (KeyValuePair<string, FileContent> item in _files)
            {
                var t = new Tuple<FileId, FileContent>(new FileId(item.Key), item.Value);
                result.Add(t);
            }

            return Task.FromResult((IEnumerable<Tuple<FileId, FileContent>>)result);
        }

        public Task<FileContent> GetAsync(FileId id)
        {
            FileContent file = null;

            // try to find the real id
            string real = KeyProvider.Find(id.Value, () => _files.Keys);

            if (real != null)
            {
                // try to get the file by using the real id
                bool succeed = _files.TryGetValue(real, out file);

                if (succeed)
                {
                    _logger.LogDebug($"Found a file by using the id {id} -> {real}.");
                }
                else
                {
                    _logger.LogError($"Did not found a file by using the id {id} -> {real}.");
                }
            }
            else
            {
                _logger.LogDebug($"Unable to find a file by using the id {id}.");
            }

            // return the file or null if no data found
            return Task.FromResult(file);
        }
    }
}
