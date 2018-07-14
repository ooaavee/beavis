using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace BeavisCli.Internal
{
    internal class JobPool
    {
        private readonly IMemoryCache _cache;

        public JobPool(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task ExecuteAsync(string key, HttpContext context, WebCliResponse response)
        {
            IJob job;

            if (_cache.TryGetValue(key, out job))
            {
                await job.ExecuteAsync(context, response);

                _cache.Remove(key);
            }
            else
            {
                // TODO: Kirjoita jotakin virhettä, koska jobia ei löytynyt!!!
            }
        }

        public string Push(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            string key = Guid.NewGuid().ToString();

            _cache.Set(key, job);

            return key;
        }
    }
}
