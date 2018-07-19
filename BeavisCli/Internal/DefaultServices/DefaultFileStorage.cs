using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultFileStorage : IFileStorage
    {
        private readonly ILogger<DefaultFileStorage> _logger;

        private readonly ConcurrentDictionary<string, UploadedFile> _files = new ConcurrentDictionary<string, UploadedFile>();


        public DefaultFileStorage(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DefaultFileStorage>();



            _files.TryAdd("aaaa", new UploadedFile() {Type = "aaaa", Name = "AAA"});
            _files.TryAdd("bbb", new UploadedFile() { Type = "bbbbbbbbbbbbb", Name = "bbbb" });
            _files.TryAdd("ccccccccc", new UploadedFile() { Type = "cc", Name = "ccccccccccccccc" });

           

            

            //var lines = ResponseRenderer.FormatLines(l, true).ToArray();
            //foreach (string line in lines)
            //{
            //    Console.WriteLine(line);
            //}
        }

        public Task<string> StoreAsync(UploadedFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            // create id for the file
            string id = ShortKey.Create(s => _files.ContainsKey(s));

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
            return Task.FromResult(id);
        }

        public Task<UploadedFile> RemoveAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            UploadedFile file = null;

            // try to find the real id
            string realId = ShortKey.FindMatching(id, () => _files.Keys);

            if (realId != null)
            {
                _logger.LogDebug($"{id} -> {realId} will be used when removing the file.");

                // try to remove the file by using the real id
                bool succeed = _files.TryRemove(realId, out file);

                if (succeed)
                {
                    _logger.LogDebug($"Successfully removed a file by using the id {id} -> {realId}.");
                }
                else
                {
                    _logger.LogError($"Failed to remove a file by using the id {id} -> {realId}.");
                }
            }
            else
            {
                _logger.LogDebug($"Unable to find a file to remove by using the id {id}.");
            }

            // return the removed file or null if we didn't remove anything
            return Task.FromResult(file);
        }

        public Task<IEnumerable<Tuple<string, UploadedFile>>> GetAllAsync()
        {
            var result = new List<Tuple<string, UploadedFile>>();

            foreach (KeyValuePair<string, UploadedFile> item in _files)
            {
                var t = new Tuple<string, UploadedFile>(item.Key, item.Value);
                result.Add(t);
            }
            
            return Task.FromResult((IEnumerable<Tuple<string, UploadedFile>>)result);
        }

    }
}
